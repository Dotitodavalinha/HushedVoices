using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class BrokenClueCleaner : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public static event Action OnAllBrokenCleaned;
    public static List<BrokenClueCleaner> allCleaners = new List<BrokenClueCleaner>();

    private bool isHovering = false;
    private ClueBoardManager board;

    [SerializeField] private GameObject breakAnimationPrefab;

    [Header("Configuración de Guardado")]
    [Tooltip("ID Único para esta pista rota, ej: 'broken_1'")]
    [SerializeField] private string brokenClueID;

    public string BrokenClueID => brokenClueID;

    private void Awake()
    {
        allCleaners.Add(this);
    }

    private void Start()
    {
        board = FindObjectOfType<ClueBoardManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (board == null || board.IsOnMainMenu) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            PlayBreakAnimation();
            board.ChangeCursor(board.hover);
        }
    }

    private void PlayBreakAnimation()
    {
        if (breakAnimationPrefab != null)
        {
            Instantiate(breakAnimationPrefab, transform.position, Quaternion.identity, transform.parent);
        }
        SoundManager.instance.PlaySound(SoundID.paperTear, false, UnityEngine.Random.Range(0.7f, 1f));

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        allCleaners.Remove(this);

        // Evita disparar el evento si el objeto se destruye por un cambio de escena
        if (gameObject.scene.isLoaded)
        {
            CheckAllCleaned();
        }
    }

    private static void CheckAllCleaned()
    {
        if (allCleaners.Count == 0)
        {
            OnAllBrokenCleaned?.Invoke();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (board == null) return;

        if (!board.IsOnMainMenu)
        {
            isHovering = true;
            GetComponent<Image>().color = Color.gray;
            board.ChangeCursor(board.deleteClues);
        }
        else
        {
            board.ChangeCursor(board.hover);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        GetComponent<Image>().color = Color.white;

        if (board != null) board.ChangeCursor(board.hover);
    }

    public void SaveState()
    {
        string uniqueID = string.IsNullOrEmpty(brokenClueID) ? gameObject.name : brokenClueID;
        if (string.IsNullOrEmpty(uniqueID)) return;

        string parentName = transform.parent.name;
        PlayerPrefs.SetString(uniqueID + "_parent", parentName);
        PlayerPrefs.Save();
    }

    public void MoveToCorcho(RectTransform newParent)
    {
        transform.SetParent(newParent, true);
    }
}