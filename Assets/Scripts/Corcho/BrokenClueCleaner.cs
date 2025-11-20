using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

// Añadimos IPointerClickHandler
public class BrokenClueCleaner : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public static event Action OnAllBrokenCleaned;
    private static List<BrokenClueCleaner> allCleaners = new List<BrokenClueCleaner>();

    private bool isHovering = false;
    private ClueBoardManager board;

    [SerializeField] private GameObject breakAnimationPrefab;

    // --- NUEVO ---
    [Header("Configuración de Guardado")]
    [Tooltip("ID Único para esta pista rota, ej: 'broken_1'")]
    [SerializeField] private string brokenClueID;

    // Propiedad pública para que el Board Manager la lea
    public string BrokenClueID { get { return brokenClueID; } }
    // --- FIN NUEVO ---


    private void Awake()
    {
        allCleaners.Add(this);
    }

    private void Start()
    {
        board = FindObjectOfType<ClueBoardManager>();
    }

    // --- MODIFICADO ---
    // Usamos esto en vez de Update() para detectar el clic
    public void OnPointerClick(PointerEventData eventData)
    {
        // Si el board no existe, o si ESTAMOS en el menú principal, no hacer nada.
        if (board == null || board.IsOnMainMenu) return;

        // Solo funciona si estamos en un panel de caso Y hacemos clic izquierdo
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            PlayBreakAnimation();
            board.ChangeCursor(board.hover);
        }
    }

    // Ya no necesitamos esto
    void Update() { }

    private void PlayBreakAnimation()
    {
        if (breakAnimationPrefab != null)
        {
            GameObject anim = Instantiate(breakAnimationPrefab, transform.position, Quaternion.identity, transform.parent);
        }
        SoundManager.instance.PlaySound(SoundID.paperTear, false, UnityEngine.Random.Range(0.7f, 1f));

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        allCleaners.Remove(this);
        CheckAllCleaned();
    }

    private static void CheckAllCleaned()
    {
        if (allCleaners.Count == 0)
        {
            OnAllBrokenCleaned?.Invoke();
            Debug.Log("Todas las pistas rotas fueron limpiadas.");
        }
    }

    // --- MODIFICADO ---
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (board == null) return;

        // Solo mostrar cursor de borrar si NO estamos en el main menu
        if (!board.IsOnMainMenu)
        {
            isHovering = true;
            GetComponent<Image>().color = Color.gray;
            board.ChangeCursor(board.deleteClues);
        }
        else
        {
            // Si estamos en el main menu, mostrar cursor 'hover' normal
            board.ChangeCursor(board.hover);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        GetComponent<Image>().color = Color.white;

        if (board != null)
            board.ChangeCursor(board.hover);
    }

    // --- NUEVAS FUNCIONES ---
    // (Igual que en ClueNode.cs)
    public void SaveState()
    {
        // Usamos el ID del campo primero. Si está vacío, usamos el nombre del objeto.
        string uniqueID = string.IsNullOrEmpty(brokenClueID) ? this.gameObject.name : brokenClueID;

        if (string.IsNullOrEmpty(uniqueID)) return;

        string parentName = transform.parent.name;
        PlayerPrefs.SetString(uniqueID + "_parent", parentName);
        PlayerPrefs.Save();
    }

    public void MoveToCorcho(RectTransform newParent)
    {
        transform.SetParent(newParent, true);
    }
    // --- FIN NUEVAS FUNCIONES ---
}