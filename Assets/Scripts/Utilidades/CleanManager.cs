using UnityEngine;
using System;
using System.Linq; // Necesitas esto para usar .Count()

public class CleanManager : MonoBehaviour
{
    public static CleanManager Instance { get; private set; }

    private int totalCleanItems;
    public int cleanedItemsCount = 0;

    public event Action OnItemCleaned;
    public event Action OnHouseCleaned;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        totalCleanItems = FindObjectsOfType<CleanItem>().Count();
        //Debug.Log($" Total de ítems de limpieza encontrados en la escena: {totalCleanItems}");
    }

    public void RegisterCleanedItem()
    {
        cleanedItemsCount++;
        //Debug.Log($" Item limpiado. Items limpiados: {cleanedItemsCount} de {totalCleanItems}");

        if (cleanedItemsCount >= totalCleanItems)
        {
            OnHouseCleaned?.Invoke();
            Debug.Log(" casa limpia");
        }
    }

    public bool IsHouseClean()
    {
        return cleanedItemsCount >= totalCleanItems;
    }
}