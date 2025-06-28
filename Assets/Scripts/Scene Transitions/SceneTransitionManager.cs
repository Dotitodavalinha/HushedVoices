using UnityEngine;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;
    public string lastEntryPointID;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void TransitionToScene(SceneTransitionData data)
    {
        lastEntryPointID = data.entryPointID;
        GameManager.Instance.LoadScene(data.targetScene);
    }
}
