using UnityEngine;

public class NPCMoodController : MonoBehaviour
{
    [Header("ID único para este NPC")]
    public string npcId;

    public SpriteRenderer spriteRenderer;
    public Sprite normalFace;
    public Sprite happyFace;
    public Sprite angryFace;

    [SerializeField] private Transform moodIconTransform;

    void Update()
    {
        if (ActiveCameraTracker.ActiveCamera != null && moodIconTransform != null)
        {
            Vector3 toCam = ActiveCameraTracker.ActiveCamera.transform.position - moodIconTransform.position;
            toCam.y = 0;
            moodIconTransform.forward = toCam.normalized;
        }
    }

    void Start()
    {
        // Recupera el mood guardado o arranca en Normal
        MoodType savedMood = NPCMoodManager.Instance.GetMood(npcId);
        switch (savedMood)
        {
            case MoodType.Happy: SetMoodHappy(); break;
            case MoodType.Angry: SetMoodAngry(true); break; // por default false
            default: SetMoodNormal(); break;
        }
    }

    public void SetMoodHappy()
    {
        spriteRenderer.sprite = happyFace;
        ParanoiaManager.Instance.SetParanoiaValue(-1f / 3f);
        SoundManager.instance.ChangeVolumeOneMusic(MusicID.StaticSound, -1f / 3f);

        NPCMoodManager.Instance.SetMood(npcId, MoodType.Happy);
    }

    public void SetMoodNormal()
    {
        spriteRenderer.sprite = normalFace;
        NPCMoodManager.Instance.SetMood(npcId, MoodType.Normal);
    }

    // Puede llamarse de dos formas: true o false
    public void SetMoodAngry(bool IsNotPolice)
    {
        spriteRenderer.sprite = angryFace;

        ParanoiaManager.Instance.SetParanoiaValue(1f / 3f);
        SoundManager.instance.ChangeVolumeOneMusic(MusicID.StaticSound, 1f / 3f);

        if (JailManager.Instance != null && IsNotPolice == true)
        {
            JailManager.Instance.Increment();
        }

        NPCMoodManager.Instance.SetMood(npcId, MoodType.Angry);
    }
}
