using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleSpawnPointResolver : MonoBehaviour, ISpawnPointResolver
{
    public static SimpleSpawnPointResolver Instance { get; private set; }

    NpcSpawnPoint[] points;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("SimpleSpawnPointResolver duplicado, destruyendo el nuevo.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        RebuildIndex();
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        // 1) Primero reconstruimos los puntos de spawn
        RebuildIndex();

        // 2) Luego pedimos al manager que recalcule presencia de NPCs
        if (NPCScheduleManager.Instance != null)
        {
            NPCScheduleManager.Instance.RefreshAll();
        }
        else
        {
            Debug.LogWarning($"SimpleSpawnPointResolver: no hay NPCScheduleManager.Instance en escena '{s.name}'.");
        }
    }

    void RebuildIndex()
    {
        points = FindObjectsOfType<NpcSpawnPoint>(true); // incluye hijos, etc.

        var sceneName = SceneManager.GetActiveScene().name;
        if (points == null || points.Length == 0)
        {
            Debug.LogWarning($"SimpleSpawnPointResolver: no se encontraron NpcSpawnPoint en la escena '{sceneName}'.");
        }
        else
        {
            Debug.Log($"SimpleSpawnPointResolver: encontrados {points.Length} NpcSpawnPoint en la escena '{sceneName}'.");
        }
    }

    public bool TryGetPoint(string sceneId, string locationId, string npcId,
                         out Vector3 pos, out Quaternion rot)
    {
        if (points == null)
        {
            Debug.LogError("SimpleSpawnPointResolver: 'points' es null. ¿RebuildIndex() no se ejecutó?");
            pos = Vector3.zero;
            rot = Quaternion.identity;
            return false;
        }

        foreach (var p in points)
        {
            if (p == null) continue;

            if (p.locationId == locationId &&
                p.gameObject.scene.name == sceneId)
            {
                pos = p.transform.position;
                rot = p.transform.rotation;  // <- acá definís hacia dónde mira el NPC
                return true;
            }
        }

        Debug.LogError(
            $"SimpleSpawnPointResolver: no se encontró spawn para npcId='{npcId}', scene='{sceneId}', locationId='{locationId}'. " +
            $"NpcSpawnPoint totales en escena actual: {points.Length}."
        );

        pos = Vector3.zero;
        rot = Quaternion.identity;
        return false;
    }

}
