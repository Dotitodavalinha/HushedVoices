using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MemoCard : MonoBehaviour, IPointerClickHandler
{
    [Header("Refs")]
    [SerializeField] private Image frontImage;
    [SerializeField] private Image backImage;

    private MemoTest_Manager manager;
    public int PairId { get; private set; }
    public bool IsRevealed { get; private set; }

    public void Setup(Sprite frontSprite, MemoTest_Manager manager, int pairId)
    {
        this.manager = manager;
        PairId = pairId;

        if (frontImage != null)
            frontImage.sprite = frontSprite;

        ShowBack();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        manager?.OnCardClicked(this);
    }

    public void Reveal()
    {
        IsRevealed = true;
        if (frontImage != null) frontImage.gameObject.SetActive(true);
        if (backImage != null) backImage.gameObject.SetActive(false);
    }

    public void ShowBack()
    {
        IsRevealed = false;
        if (frontImage != null) frontImage.gameObject.SetActive(false);
        if (backImage != null) backImage.gameObject.SetActive(true);
    }
}
