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
        // Al inicio, solo aplicamos el mood guardado sin modificar la paranoia
        ApplySavedMood();
    }

    public void SetMoodHappy()
    {
        spriteRenderer.sprite = happyFace;
        // La paranoia se modifica solo en el momento del cambio
        ParanoiaManager.Instance.ModifyParanoia(-0.33f);
        SoundManager.instance.ChangeVolumeOneMusic(MusicID.StaticSound, -1f / 3f);

        NPCMoodManager.Instance.SetMood(npcId, MoodType.Happy);
    }

    public void SetMoodNormal()
    {
        spriteRenderer.sprite = normalFace;
        NPCMoodManager.Instance.SetMood(npcId, MoodType.Normal);
    }

    public void SetMoodAngry(bool IsNotPolice)
    {
        spriteRenderer.sprite = angryFace;

        // La paranoia se modifica solo en el momento del cambio
        ParanoiaManager.Instance.ModifyParanoia(1f / 3f);
        SoundManager.instance.ChangeVolumeOneMusic(MusicID.StaticSound, 1f / 3f);

        if (JailManager.Instance != null && IsNotPolice == true)
        {
            JailManager.Instance.Increment();
        }

        NPCMoodManager.Instance.SetMood(npcId, MoodType.Angry);
    }

    // Nuevo método para aplicar el estado de ánimo guardado
    private void ApplySavedMood()
    {
        MoodType savedMood = NPCMoodManager.Instance.GetMood(npcId);
        switch (savedMood)
        {
            case MoodType.Happy:
                spriteRenderer.sprite = happyFace;
                break;
            case MoodType.Angry:
                spriteRenderer.sprite = angryFace;
                break;
            default:
                spriteRenderer.sprite = normalFace;
                break;
        }
    }
}