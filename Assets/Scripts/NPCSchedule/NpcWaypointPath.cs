using System.Collections.Generic;
using UnityEngine;

public class NpcWaypointPath : MonoBehaviour
{
    [Tooltip("Debe matchear con el npcId del NPCDespawn / NPCScheduleManager")]
    public string npcId;

    [Tooltip("Waypoints de salida en orden")]
    public List<Transform> points = new List<Transform>();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (int i = 0; i < points.Count; i++)
        {
            if (points[i] == null) continue;
            Gizmos.DrawSphere(points[i].position, 0.1f);

            if (i + 1 < points.Count && points[i + 1] != null)
                Gizmos.DrawLine(points[i].position, points[i + 1].position);
        }
    }
}
