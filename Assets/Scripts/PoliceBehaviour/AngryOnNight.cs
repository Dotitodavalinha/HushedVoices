using UnityEngine;

public class AngryOnNight : MonoBehaviour
{
    private NightManager nightManager;
    private NPCMoodController moodController;
    private bool forcedByNight = false;
    private MoodType previousMood;
    private bool initialized = false;

    private void Start()
    {
        nightManager = FindObjectOfType<NightManager>();
        moodController = GetComponent<NPCMoodController>();
        initialized = nightManager != null && moodController != null;
    }

    private void Update()
    {
        if (!initialized) return;

        if (nightManager.IsNight)
        {
            if (!forcedByNight)
            {
                var mgr = NPCMoodManager.Instance;
                if (mgr != null)
                {
                    previousMood = mgr.GetMood(moodController.npcId);
                }
                else
                {
                    previousMood = MoodType.Normal;
                }

                if (previousMood != MoodType.Angry)
                {
                    moodController.SetMoodAngry(false); // seteo mood Angry, el False es para indicar q el npc es un POLICIA.
                    forcedByNight = true;
                }
            }
            return;
        }

        if (forcedByNight)
        {
            switch (previousMood)
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
            forcedByNight = false;
        }
    }
}
