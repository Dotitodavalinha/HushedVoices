using System.Collections.Generic;
using UnityEngine;

public class MemoTest_Manager : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private int columns = 3;
    [SerializeField] private int rows = 2;
    [SerializeField] private RectTransform boardParent; // padre dentro del Canvas (ideal con GridLayoutGroup)

    [Header("Card Prefabs (caras del memotest)")]
    [SerializeField] private List<GameObject> cardPrefabs = new List<GameObject>();

    [Header("Debug")]
    [SerializeField] private bool spawnOnStart = false;

    private void Start()
    {
        if (spawnOnStart)
            SetupBoard();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) 
        {
            SetupBoard();
        }
    }

    private void SetupBoard()
    {
        if (boardParent == null)
        {
            Debug.LogError("[MemoTest] Falta asignar boardParent.");
            return;
        }

        int totalSlots = columns * rows;
        if (totalSlots % 2 != 0)
        {
            Debug.LogError("[MemoTest] La grilla debe tener cantidad PAR de casillas.");
            return;
        }

        int pairCount = totalSlots / 2;
        if (cardPrefabs.Count < pairCount)
        {
            Debug.LogError($"[MemoTest] Necesitás al menos {pairCount} prefabs distintos (uno por pareja).");
            return;
        }

        // Limpiar tablero anterior
        for (int i = boardParent.childCount - 1; i >= 0; i--)
        {
            Destroy(boardParent.GetChild(i).gameObject);
        }

        // Armar lista de cartas a spawnear (2 de cada prefab)
        List<GameObject> cardsToSpawn = new List<GameObject>(totalSlots);
        for (int i = 0; i < pairCount; i++)
        {
            GameObject prefab = cardPrefabs[i];
            cardsToSpawn.Add(prefab);
            cardsToSpawn.Add(prefab);
        }

        // Mezclar posiciones
        Shuffle(cardsToSpawn);

        // Instanciar en el parent (GridLayoutGroup se encarga de acomodar en matriz)
        foreach (GameObject prefab in cardsToSpawn)
        {
            Instantiate(prefab, boardParent);
        }
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
}
