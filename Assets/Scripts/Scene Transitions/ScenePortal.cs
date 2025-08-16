using UnityEngine;

public class ScenePortal : MonoBehaviour
{
    [SerializeField] private string targetSceneName;
    [SerializeField] private string entryPointIDInTargetScene;

    [SerializeField] private LightingManager timeManager;
    [SerializeField] private GameObject objectToActivate;

    private void Update()
    {
        if (timeManager.TimeOfDay >= 20f || timeManager.TimeOfDay < 5f)
        {
            objectToActivate.SetActive(true);
        }
        else
        {
            objectToActivate.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        SceneTransitionData data = new SceneTransitionData(targetSceneName, entryPointIDInTargetScene);
        SceneTransitionManager.Instance.TransitionToScene(data);
    }
}
