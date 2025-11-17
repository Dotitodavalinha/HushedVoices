using UnityEngine;

public class FolioManager : MonoBehaviour
{
    private const string UNLOCKED_KEY = "FolioUnlocked";

    [SerializeField] private GameObject folioObject;

    private bool allBrokenCleaned = false;

    void Awake()
    {
        allBrokenCleaned = PlayerPrefs.GetInt(UNLOCKED_KEY, 0) == 1;

        if (folioObject != null)
        {
            folioObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        BrokenClueCleaner.OnAllBrokenCleaned += OnBrokenCleaned;
    }

    private void OnDisable()
    {
        BrokenClueCleaner.OnAllBrokenCleaned -= OnBrokenCleaned;
    }

    private void OnBrokenCleaned()
    {
        allBrokenCleaned = true;

        PlayerPrefs.SetInt(UNLOCKED_KEY, 1);
        PlayerPrefs.Save();

        if (folioObject != null)
        {
            folioObject.SetActive(true);
        }
    }

    public void ShowFolioIfReady()
    {
        if (allBrokenCleaned && folioObject != null)
        {
            folioObject.SetActive(true);
        }
    }

    public void HideFolio()
    {
        if (folioObject != null)
        {
            folioObject.SetActive(false);
        }
    }
}