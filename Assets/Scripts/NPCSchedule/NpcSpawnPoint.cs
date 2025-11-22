using UnityEngine;

public class NpcSpawnPoint : MonoBehaviour
{
    [Tooltip("Debe matchear con Block.locationId en el NPCScheduleManager")]
    public string locationId;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}
