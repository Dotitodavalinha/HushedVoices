using UnityEngine;

public class AngryOnNight : MonoBehaviour
{
    private NightManager nightManager;
    private NPCMoodController moodController;
    private bool alreadyAngry = false;

    private void Start()
    {
        nightManager = FindObjectOfType<NightManager>();
        moodController = GetComponent<NPCMoodController>();
    }

    private void Update()
    {
        if (nightManager == null || moodController == null) return;

        if (!nightManager.IsNight)
        {
            alreadyAngry = false;
            return;
        }

        if (!alreadyAngry)
        {
            moodController.SetMoodAngry(false);
            alreadyAngry = true;
        }
    }

}
