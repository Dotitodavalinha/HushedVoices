using UnityEngine;

public class ScenePortal : MonoBehaviour
{
    [SerializeField] private string targetSceneName;
    [SerializeField] private string entryPointIDInTargetScene;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        SceneTransitionData data = new SceneTransitionData(targetSceneName, entryPointIDInTargetScene);
        SceneTransitionManager.Instance.TransitionToScene(data);
    }
}
