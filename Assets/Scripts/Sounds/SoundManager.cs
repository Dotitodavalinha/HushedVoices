using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public Slider volumeSFXSlider;
    public Slider volumeMusicSlider;
    public AudioClip[] sounds;
    public AudioClip[] music;

    [HideInInspector] public AudioSource[] sfxChannel;
    [HideInInspector] public AudioSource[] musicChannel;

    [Range(0, 1)] public float volumeSFX;
    [Range(0, 1)] public float volumeMusic;

    public static SoundManager instance;

    private MusicID currentMusic = MusicID.None;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        sfxChannel = new AudioSource[sounds.Length];
        for (int i = 0; i < sfxChannel.Length; i++)
        {
            sfxChannel[i] = gameObject.AddComponent<AudioSource>();
            sfxChannel[i].clip = sounds[i];
        }

        musicChannel = new AudioSource[music.Length];
        for (int i = 0; i < musicChannel.Length; i++)
        {
            musicChannel[i] = gameObject.AddComponent<AudioSource>();
            musicChannel[i].clip = music[i];
        }

      
    }
    void Start()
    {
        if (PlayerPrefs.HasKey("volumeMusic"))
        LoadVolume();
        else
        {
            PlayerPrefs.SetFloat("volumeMusic", 1);
            PlayerPrefs.SetFloat("volumeSFX", 1);
            LoadVolume();
        }
    }


    public void SetVolume()
    {
        SoundManager.instance.ChangeVolumeMusic(volumeMusicSlider.value);
        SoundManager.instance.ChangeVolumeSound(volumeSFXSlider.value);
        SaveVolume();
    }

    public void SaveVolume()
    {
        PlayerPrefs.SetFloat("volumeSFX", volumeSFXSlider.value);
        PlayerPrefs.SetFloat("volumeMusic", volumeMusicSlider.value);
    }

    public void LoadVolume()
    {
        volumeSFXSlider.value = PlayerPrefs.GetFloat("volumeSFX");
        volumeMusicSlider.value = PlayerPrefs.GetFloat("volumeMusic");
    }
    #region SFX

    public bool isSoundPlaying(SoundID id)
    {
        return sfxChannel[(int)id].isPlaying;
    }

    public void PlaySound(SoundID id, bool loop = false, float pitch = 1)
    {
        sfxChannel[(int)id].loop = loop;
        sfxChannel[(int)id].volume = volumeSFX;
        sfxChannel[(int)id].pitch = pitch;
        sfxChannel[(int)id].Play();
        

    }

    public void StopAllSounds()
    {
        for (int i = 0; i < sfxChannel.Length; i++)
        {
            sfxChannel[i].Stop();
        }
    }

    public void PauseAllSounds()
    {
        for (int i = 0; i < sfxChannel.Length; i++)
        {
            sfxChannel[i].Pause();
        }
    }

    public void ResumeAllSounds()
    {
        for (int i = 0; i < sfxChannel.Length; i++)
        {
            sfxChannel[i].UnPause();
        }
    }

    public void StopSound(SoundID id)
    {
        sfxChannel[(int)id].Stop();
    }

    public void PauseSound(SoundID id)
    {
        sfxChannel[(int)id].Pause();
    }

    public void ResumeSound(SoundID id)
    {
        sfxChannel[(int)id].UnPause();
    }

    public void ToggleMuteSound(SoundID id)
    {
        sfxChannel[(int)id].mute = !sfxChannel[(int)id].mute;
    }

    public void ChangeVolumeSound(float volume)
    {
        volumeSFX = volume;
        for (int i = 0; i < sfxChannel.Length; i++)
        {
            sfxChannel[i].volume = volumeSFX;
        }

        
    }

    #endregion

    #region MUSIC

    public bool isMusicPlaying(MusicID id)
    {
        return musicChannel[(int)id].isPlaying;
    }

    public void PlayMusic(MusicID id, bool loop = false, float pitch = 1, float volumeMusic = 1)
    {
        musicChannel[(int)id].loop = loop;
        musicChannel[(int)id].volume = volumeMusic;
        musicChannel[(int)id].pitch = pitch;
        musicChannel[(int)id].Play();
        
    }

    public void ChangeMusic(MusicID newMusic, bool loop = true)
    {
        if (currentMusic == newMusic)
            return;

        if (currentMusic != MusicID.None)
            StopMusic(currentMusic);

        PlayMusic(newMusic, loop);
        currentMusic = newMusic;
    }

    public void StopAllMusic()
    {
        for (int i = 0; i < musicChannel.Length; i++)
        {
            musicChannel[i].Stop();
        }
    }

    public void PauseAllMusic()
    {
        for (int i = 0; i < musicChannel.Length; i++)
        {
            musicChannel[i].Pause();
        }
    }

    public void ResumeAllMusic()
    {
        for (int i = 0; i < musicChannel.Length; i++)
        {
            musicChannel[i].UnPause();
        }
    }

    public void StopMusic(MusicID id)
    {
        musicChannel[(int)id].Stop();
    }

    public void PauseMusic(MusicID id)
    {
        musicChannel[(int)id].Pause();
    }

    public void ResumeMusic(MusicID id)
    {
        musicChannel[(int)id].UnPause();
    }

    public void ToggleMuteMusic(MusicID id)
    {
        musicChannel[(int)id].mute = !musicChannel[(int)id].mute;
    }

    public void ChangeVolumeMusic(float volume)
    {
        volumeMusic = volume;
        for (int i = 0; i < musicChannel.Length; i++)
        {
            musicChannel[i].volume = volumeMusic;
        }

       
    }
    public void ChangeVolumeOneMusic(MusicID id, float volume)
    {
        volumeMusic += volume;
        if (volumeMusic < 0f)
        {
            volumeMusic = 0;
        }

        else if (volumeMusic > 1f)
        {
            volumeMusic = 1;
        }
        musicChannel[(int)id].volume = volumeMusic;


    }

    #endregion
}

public enum SoundID
{
    Step1Sound,
    Step2Sound,
    PageTurnSound,
    DialogueTypingSound,
    DialogueTypingHighSound,
    DialogueTypingLowSound,
    DialogueOptionSound,
    CluePickupSound,
    BookOpenSound,
    alarm,
    camChange,
}

public enum MusicID
{
    None,
    DaySound,
    NightSound,
    StaticSound,
    AmbientBreathingSound,
}