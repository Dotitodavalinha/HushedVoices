// AngryOnNight.cs (reemplazar archivo entero)
using UnityEngine;

public class AngryOnNight : MonoBehaviour
{
    private NightManager nightManager;
    private NPCMoodController moodController;
    private bool initialized = false;

    private void Start()
    {
        nightManager = FindObjectOfType<NightManager>();
        moodController = GetComponent<NPCMoodController>();
        initialized = nightManager != null && moodController != null;
        if (!initialized) return;

        if (NPCMoodManager.Instance != null)
        {
            if (NPCMoodManager.Instance.TryGetForcedPreviousMood(moodController.npcId, out MoodType prev))
            {
                if (nightManager.IsNight)
                {
                    if (NPCMoodManager.Instance.GetMood(moodController.npcId) != MoodType.Angry)
                    {
                        moodController.SetMoodAngry(false);
                    }
                }
                else
                {
                    RestorePrevious(prev);
                    NPCMoodManager.Instance.ClearForcedPreviousMood(moodController.npcId);
                }
            }
        }
    }

    private void Update()
    {
        if (!initialized) return;

        if (nightManager.IsNight)
        {
            if (NPCMoodManager.Instance != null)
            {
                MoodType current = NPCMoodManager.Instance.GetMood(moodController.npcId);
                if (current != MoodType.Angry && !NPCMoodManager.Instance.HasForcedPreviousMood(moodController.npcId))
                {
                    NPCMoodManager.Instance.SetForcedPreviousMood(moodController.npcId, current);
                    moodController.SetMoodAngry(false);
                }
            }
            else
            {
                if (NPCMoodManager.Instance.GetMood(moodController.npcId) != MoodType.Angry)
                {
                    moodController.SetMoodAngry(false);
                }
            }
            return;
        }

        if (NPCMoodManager.Instance != null && NPCMoodManager.Instance.TryGetForcedPreviousMood(moodController.npcId, out MoodType prevMood))
        {
            RestorePrevious(prevMood);
            NPCMoodManager.Instance.ClearForcedPreviousMood(moodController.npcId);
        }
    }

    private void RestorePrevious(MoodType prev)
    {
        switch (prev)
        {
            case MoodType.Happy:
                moodController.SetMoodHappy();
                break;
            case MoodType.Normal:
                moodController.SetMoodNormal();
                break;
            default:
                moodController.SetMoodAngry(false);
                break;
        }
    }
}
