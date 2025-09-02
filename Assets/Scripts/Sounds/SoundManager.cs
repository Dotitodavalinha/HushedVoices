using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
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
        if (!PlayerPrefs.HasKey("volumeMusic"))
        {
            PlayerPrefs.SetFloat("volumeMusic", 1f);
            PlayerPrefs.SetFloat("volumeSFX", 1f);
            PlayerPrefs.Save();
        }
        LoadVolume();
    }

    public void SaveVolume()
    {
        PlayerPrefs.SetFloat("volumeSFX", volumeSFX);
        PlayerPrefs.SetFloat("volumeMusic", volumeMusic);
        PlayerPrefs.Save();
    }

    public void LoadVolume()
    {
        volumeSFX = PlayerPrefs.GetFloat("volumeSFX", 1f);
        volumeMusic = PlayerPrefs.GetFloat("volumeMusic", 1f);

        // Aplicar a los canales existentes
        ChangeVolumeSound(volumeSFX);
        ChangeVolumeMusic(volumeMusic);
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


    // Cambiá la firma para no depender de un parámetro de volumen externo
    public void PlayMusic(MusicID id, bool loop = false, float pitch = 1f)
    {
        musicChannel[(int)id].loop = loop;
        musicChannel[(int)id].volume = volumeMusic; // usar el volumen global actual
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
    public void ChangeVolumeOneMusic(MusicID id, float delta)
    {
        var channel = musicChannel[(int)id];

        // agarramos el volumen actual de ese canal y le sumamos/restamos
        float newVolume = Mathf.Clamp(channel.volume + delta, 0f, 1f);

        channel.volume = newVolume;
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