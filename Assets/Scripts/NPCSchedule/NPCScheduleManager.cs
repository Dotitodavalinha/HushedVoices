using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[System.Flags] public enum DayMask { Mon = 1, Tue = 2, Wed = 4, Thu = 8, Fri = 16, Sat = 32, Sun = 64, All = 127 }

public interface ISpawnPointResolver
{
    // Devuelve posición y rotación para (sceneId, locationId, npcId).
    bool TryGetPoint(string sceneId, string locationId, string npcId,
                     out Vector3 pos, out Quaternion rot);
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

    bool ResolveSpawn(string sceneId, string locationId, string npcId,
                   out Vector3 pos, out Quaternion rot)
    {
        if (resolver == null)
        {
            Debug.LogError(
                $"NPCScheduleManager: no hay ISpawnPointResolver asignado, no se puede resolver spawn para npcId='{npcId}', " +
                $"scene='{sceneId}', locationId='{locationId}'."
            );
            pos = Vector3.zero;
            rot = Quaternion.identity;
            return false;
        }

        bool ok = resolver.TryGetPoint(sceneId, locationId, npcId, out pos, out rot);
        if (!ok)
        {
            // El resolver ya logueó un error concreto
            return false;
        }
        return true;
    }


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
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("NPCScheduleManager duplicado, destruyendo el nuevo.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        resolver = spawnPointResolverMB as ISpawnPointResolver;
        if (resolver == null && spawnPointResolverMB != null)
        {
            Debug.LogError("NPCScheduleManager: el MonoBehaviour asignado en 'spawnPointResolverMB' NO implementa ISpawnPointResolver.");
        }
    }

    void Start()
    {
        RefreshAll(); // primer cálculo al iniciar
    }
    private void Update()
    {
        // DEBUG: refreshear a mano con la Y
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Debug.Log("NPCScheduleManager: RefreshAll() manual por tecla Y.");
            RefreshAll();
        }
    }

    public void RefreshAll()
    {
        string sceneId = SceneManager.GetActiveScene().name;
        int hour = GetCurrentHour();
        DayMask today = GetTodayMask();

        Debug.Log($"NPCScheduleManager.RefreshAll -> Scene='{sceneId}', DayMask='{today}', Hour={hour}");

        for (int i = 0; i < npcs.Count; i++)
        {
            var e = npcs[i];

            if (e.prefab == null)
            {
                Debug.LogError($"NPCScheduleManager: NpcEntry index {i} (npcId='{e.npcId}') no tiene prefab asignado.");
                continue;
            }

            int blockIndex = GetActiveBlockIndex(e.schedule, today, hour);
            active.TryGetValue(e.npcId, out var inst);
            // justo después de: active.TryGetValue(e.npcId, out var inst);
            if (inst != null && inst.go == null)
            {
                // por si en algún momento lo destruye otro script
                active.Remove(e.npcId);
            }
            else if (inst != null)
            {
                var despawn = inst.go.GetComponent<NPCDespawn>();
                if (despawn != null && despawn.HasFinishedLeaving)
                {
                    // Ya terminó la ruta y está en modo fantasma: ahora sí lo limpiamos del diccionario.
                    Debug.Log($"NPCScheduleManager: limpieza post-ruta de npcId='{e.npcId}'.");
                    Destroy(inst.go);
                    active.Remove(e.npcId);
                    inst = null;
                }
            }

            bool shouldBeHere = blockIndex >= 0 && e.schedule[blockIndex].sceneId == sceneId;

            if (shouldBeHere)
            {
                var b = e.schedule[blockIndex];

                // Resolver posición + rotación
                Vector3 pos;
                Quaternion rot;
                bool hasSpawn = ResolveSpawn(sceneId, b.locationId, e.npcId, out pos, out rot);

                if (!hasSpawn)
                {
                    // Ya se logueó el error en ResolveSpawn
                    continue;
                }

                if (inst == null)
                {
                    // Spawn nuevo
                    var go = Instantiate(e.prefab, pos, rot);
                    active[e.npcId] = new ActiveNpc
                    {
                        npcId = e.npcId,
                        go = go,
                        lastBlockIndex = blockIndex,
                        lastLocationId = b.locationId
                    };
                    Debug.Log($"NPCScheduleManager: instanciado npcId='{e.npcId}' en scene='{sceneId}', locationId='{b.locationId}'.");
                }
                else
                {
                    // Ya existe: si cambió de bloque o de location, reubicar y reorientar
                    if (inst.lastBlockIndex != blockIndex || inst.lastLocationId != b.locationId)
                    {
                        inst.lastBlockIndex = blockIndex;
                        inst.lastLocationId = b.locationId;
                        inst.go.transform.SetPositionAndRotation(pos, rot);
                        Debug.Log($"NPCScheduleManager: movido npcId='{e.npcId}' a nueva locationId='{b.locationId}' en scene='{sceneId}'.");
                    }
                }
            }

            else
            {
                if (inst != null)
                {
                    var despawn = inst.go.GetComponent<NPCDespawn>();

                    if (despawn != null && despawn.HasWaypoints && !despawn.HasFinishedLeaving)
                    {
                        // Le decimos que se vaya caminando en lugar de desaparecer
                        Debug.Log($"NPCScheduleManager: npcId='{e.npcId}' debería irse, iniciando ruta de salida.");
                        despawn.StartLeavingFromSchedule();
                        // NO lo destruimos todavía, dejamos que termine la ruta.
                    }
                    else
                    {
                        Debug.Log($"NPCScheduleManager: destruyendo npcId='{e.npcId}' (no corresponde en esta escena/horario o no tiene ruta).");
                        Destroy(inst.go);
                        active.Remove(e.npcId);
                    }
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

                if (hour >= b.startHour || hour < b.endHour) return i;
            }
        }
        return -1;
    }


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
