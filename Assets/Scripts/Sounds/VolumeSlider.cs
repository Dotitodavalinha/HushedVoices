using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public enum SliderType { Music, SFX }
    public SliderType type;

    Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();

        // sincroniza el valor visual al entrar a la escena
        if (SoundManager.instance != null)
        {
            slider.SetValueWithoutNotify(
                type == SliderType.Music ? SoundManager.instance.volumeMusic
                                         : SoundManager.instance.volumeSFX
            );
        }

        slider.onValueChanged.AddListener(OnValueChanged);
        // Rango recomendado en el inspector: Min 0, Max 1
    }

    void OnDestroy()
    {
        if (slider != null)
            slider.onValueChanged.RemoveListener(OnValueChanged);
    }

    void OnValueChanged(float value)
    {
        if (SoundManager.instance == null) return;

        if (type == SliderType.Music)
            SoundManager.instance.ChangeVolumeMusic(value);
        else
            SoundManager.instance.ChangeVolumeSound(value);

        SoundManager.instance.SaveVolume();
    }
}
