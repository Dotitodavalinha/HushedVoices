using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Cat : MonoBehaviour
{
    public AudioClip meowClip;
    public float minTime = 5f;
    public float maxTime = 12f;

    private float minDistance = 5f;
    private float maxDistance = 20f;
    public float volumeMultiplier = 8f;

    private AudioSource catAudio;

    private void Start()
    {
        catAudio = GetComponent<AudioSource>();

        catAudio.spatialBlend = 1f;

        catAudio.rolloffMode = AudioRolloffMode.Linear;

        catAudio.minDistance = minDistance;
        catAudio.maxDistance = maxDistance;

        StartCoroutine(MeowRoutine());
    }

    private IEnumerator MeowRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(waitTime);

            if (meowClip != null)
            {
                catAudio.pitch = Random.Range(0.9f, 1.1f);

                float globalVolume = SoundManager.instance != null ? SoundManager.instance.volumeSFX : 1f;
                float finalVolume = globalVolume * volumeMultiplier;

                catAudio.PlayOneShot(meowClip, finalVolume);
            }
        }
    }
}