using UnityEngine;
using UnityEngine.UI;

public class ClueNode : MonoBehaviour
{
    public ClueData data;
    public Image image;
    public GameObject missingOverlay;

    public RectTransform RectTransform { get; private set; }

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
    }

    public void Init(ClueData clue, bool isFound)
    {
        data = clue;
        image.sprite = clue.clueSprite;
        missingOverlay.SetActive(!isFound);
    }
}
