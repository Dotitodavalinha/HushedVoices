using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;


public class BrokenClueCleaner : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static event Action OnAllBrokenCleaned;

    private static List<BrokenClueCleaner> allCleaners = new List<BrokenClueCleaner>();

    private bool isHovering = false;
    private ClueBoardManager board;

    [SerializeField] private GameObject breakAnimationPrefab;

    private void Awake()
    {
        allCleaners.Add(this);
    }

    private void Start()
    {
        board = FindObjectOfType<ClueBoardManager>();
    }

    void Update()
    {
        if (isHovering && Input.GetMouseButtonDown(0))
        {
            PlayBreakAnimation();
            if (board != null)
                board.ChangeCursor(board.hover);
        }
    }

    private void PlayBreakAnimation()
    {
        if (breakAnimationPrefab != null)
        {
            GameObject anim = Instantiate(breakAnimationPrefab,transform.position,Quaternion.identity,transform.parent);
        }

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        GetComponent<Image>().color = Color.gray;

        if (board != null)
            board.ChangeCursor(board.deleteClues);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        GetComponent<Image>().color = Color.white;

        if (board != null)
            board.ChangeCursor(board.hover);
    }
}
