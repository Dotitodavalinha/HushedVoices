using UnityEngine;

public class PlayerSpawnPositionSetter : MonoBehaviour
{
    void Start()
    {
        string entryPoint = SceneTransitionManager.Instance.lastEntryPointID;
        if (!string.IsNullOrEmpty(entryPoint))
        {
            Transform spawn = GameObject.Find(entryPoint)?.transform;
            if (spawn != null)
            {
                transform.position = spawn.position;
            }
            else
            {
                Debug.LogWarning("No se encontró el punto de entrada: " + entryPoint);
            }
        }
    }
}
