using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClueZoomUI : MonoBehaviour
{
    public static ClueZoomUI Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private Image clueImage;
    [SerializeField] private TextMeshProUGUI clueInfo;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void ShowClue(ClueData clue)
    {
        panel.SetActive(true);
        clueImage.sprite = clue.clueSprite;
        clueInfo.text = $"Pista: {clue.clueID}";
    }

    public void CloseClue()
    {
        panel.SetActive(false);
    }
}
