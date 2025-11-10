using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[System.Flags] public enum DayMask { Mon = 1, Tue = 2, Wed = 4, Thu = 8, Fri = 16, Sat = 32, Sun = 64, All = 127 }

public interface ISpawnPointResolver
{
    // Devuelve una posición para (sceneId, locationId, npcId). Si no hay, false.
    bool TryGetPoint(string sceneId, string locationId, string npcId, out Vector3 pos);
}

public class NPCScheduleManager : MonoBehaviour
{
    public static NPCScheduleManager Instance { get; private set; }

    [System.Serializable]
    public struct Block
    {
        public DayMask days;                     // ej: DayMask.All
        [Range(0, 24)] public int startHour;      // 0..24
        [Range(0, 24)] public int endHour;        // exclusivo (si start=8, end=12 => [8..11])
        public string sceneId;                   // nombre EXACTO de la escena
        public string locationId;                // id lógico dentro de la escena
    }

    [System.Serializable]
    public struct NpcEntry
    {
        public string npcId;
        public GameObject prefab;
        public List<Block> schedule;             // ahora por horas, sin SO
    }

    [Header("Datos")]
    [SerializeField] private List<NpcEntry> npcs = new();

    [Header("Opcional")]
    [Tooltip("Resolver de spawnpoints (puede quedar vacío).")]
    public MonoBehaviour spawnPointResolverMB;   // Debe implementar ISpawnPointResolver

    ISpawnPointResolver resolver;

    class ActiveNpc
    {
        public string npcId;
        public GameObject go;
        public int lastBlockIndex = -1;
        public string lastLocationId;
    }
    readonly Dictionary<string, ActiveNpc> active = new(); // npcId -> instancia

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        resolver = spawnPointResolverMB as ISpawnPointResolver; // puede ser null
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        // Cuando tengas un evento “OnHourChanged”, llamá RefreshAll() desde ahí.
        // Mientras, podés llamarlo manualmente después de cambiar la hora.
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        RefreshAll(); // primer cálculo al iniciar
    }

    void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        RefreshAll();
    }

    /// <summary>Forzá recalcular spawns (llamalo cuando cambie la hora/día).</summary>
    public void RefreshAll()
    {
        string sceneId = SceneManager.GetActiveScene().name;
        int hour = GetCurrentHour();
        DayMask today = GetTodayMask();

        for (int i = 0; i < npcs.Count; i++)
        {
            var e = npcs[i];
            int blockIndex = GetActiveBlockIndex(e.schedule, today, hour);

            bool shouldBeHere = blockIndex >= 0 && e.schedule[blockIndex].sceneId == sceneId;
            active.TryGetValue(e.npcId, out var inst);

            if (shouldBeHere)
            {
                var b = e.schedule[blockIndex];

                if (inst == null)
                {
                    Vector3 pos = ResolveSpawn(sceneId, b.locationId, e.npcId);
                    var go = Instantiate(e.prefab, pos, Quaternion.identity);
                    active[e.npcId] = new ActiveNpc { npcId = e.npcId, go = go, lastBlockIndex = blockIndex, lastLocationId = b.locationId };
                }
                else
                {
                    if (inst.lastBlockIndex != blockIndex || inst.lastLocationId != b.locationId)
                    {
                        inst.lastBlockIndex = blockIndex;
                        inst.lastLocationId = b.locationId;
                        Vector3 pos = ResolveSpawn(sceneId, b.locationId, e.npcId);
                        inst.go.transform.position = pos;
                    }
                }
            }
            else
            {
                if (inst != null)
                {
                    Destroy(inst.go);
                    active.Remove(e.npcId);
                }
            }
        }
    }

    int GetActiveBlockIndex(List<Block> blocks, DayMask today, int hour)
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            var b = blocks[i];
            if ((b.days & today) == 0) continue;

            // ventana [startHour, endHour) en horas enteras
            // soporta casos end < start (cruce de medianoche), si lo necesitás:
            if (b.startHour <= b.endHour)
            {
                if (hour >= b.startHour && hour < b.endHour) return i;
            }
            else
            {
                // ej: 22 -> 3 (pasa por medianoche)
                if (hour >= b.startHour || hour < b.endHour) return i;
            }
        }
        return -1;
    }

    Vector3 ResolveSpawn(string sceneId, string locationId, string npcId)
    {
        if (resolver != null && resolver.TryGetPoint(sceneId, locationId, npcId, out var pos))
            return pos;
        return Vector3.zero; // placeholder
    }

    // ======= Adaptadores a tus singletons =======
    int GetCurrentHour()
    {
        // Usa DaysManager.CurrentHour (deriva de LightingManager.TimeOfDay 0..24)
        return DaysManager.Instance != null ? DaysManager.Instance.CurrentHour : 8;
    }

    DayMask GetTodayMask()
    {
        // Map simple: currentDay % 7  -> DayMask
        if (DaysManager.Instance == null) return DayMask.Mon;
        int idx = DaysManager.Instance.CurrentDay % 7;
        return (DayMask)(1 << idx); // 0=Mon, 1=Tue, ...
    }
}
