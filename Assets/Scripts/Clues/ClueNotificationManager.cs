using System.Collections.Generic;
using UnityEngine;

public class ClueNotificationManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private ClueDatabase _clueDatabase;
    [SerializeField] private GameObject _notificationPrefab;
    [SerializeField] private Transform _container;

    public static ClueNotificationManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        PlayerClueTracker.OnCluesLost += HandleCluesLost;
        PlayerClueTracker.OnCluesAdded += HandleCluesAdded; // AÑADIDO
    }

    private void OnDisable()
    {
        PlayerClueTracker.OnCluesLost -= HandleCluesLost;
        PlayerClueTracker.OnCluesAdded -= HandleCluesAdded; // AÑADIDO
    }

    private void HandleCluesLost(List<string> lostClueIDs)
    {
        if (_clueDatabase == null)
        {
            Debug.LogError("ERROR: ¡La ClueDatabase no está asignada en el Inspector!");
            return;
        }

        foreach (string id in lostClueIDs)
        {
            Sprite icon = _clueDatabase.GetSpriteByID(id);

            if (icon == null)
            {
                Debug.LogWarning($"NO SE ENCONTRÓ el sprite para '{id}'. No se creará prefab.");
                continue;
            }

            GameObject notificationGO = Instantiate(_notificationPrefab, _container);

            ClueNotificationItem item = notificationGO.GetComponent<ClueNotificationItem>();
            if (item != null)
            {
                item.Initialize(icon, "-1 Pista");
            }
        }
    }

    // --- FUNCIÓN NUEVA AÑADIDA ---
    private void HandleCluesAdded(List<string> addedClueIDs)
    {
        if (_clueDatabase == null)
        {
            Debug.LogError("ERROR: ¡La ClueDatabase no está asignada en el Inspector!");
            return;
        }

        foreach (string id in addedClueIDs)
        {
            Sprite icon = _clueDatabase.GetSpriteByID(id);

            if (icon == null)
            {
                Debug.LogWarning($"NO SE ENCONTRÓ el sprite para '{id}'. No se creará prefab.");
                continue;
            }

            GameObject notificationGO = Instantiate(_notificationPrefab, _container);

            ClueNotificationItem item = notificationGO.GetComponent<ClueNotificationItem>();
            if (item != null)
            {
                item.Initialize(icon, "+1 Pista"); // MODIFICADO EL TEXTO
            }
        }
    }
}