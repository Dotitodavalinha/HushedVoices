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
    public GameObject culpablesPanel;


    [Header("Botones de interacción")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button culpableButton;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
        closeButton.onClick.AddListener(CloseClue);
        culpableButton.onClick.AddListener(DeclareCulpable);
    }

    public void ShowClue(ClueData clue)
    {
        panel.SetActive(true);
        clueImage.sprite = clue.clueSprite;
        clueImage.preserveAspect = true;
        clueInfo.text = $"Pista: {clue.clueID}";
        culpableButton.gameObject.SetActive(false);
        GameManager.Instance.SetBlockEInput(true);
    }

    public void ShowImage(Sprite sprite)
    {
        panel.SetActive(true);
        clueImage.sprite = sprite;
        clueImage.preserveAspect = true;
        clueInfo.text = "Imagen sin ID";
        culpableButton.gameObject.SetActive(true);
        GameManager.Instance.SetBlockEInput(true);
    }


    public void CloseClue()
    {
        panel.SetActive(false);

        if (culpablesPanel != null && culpablesPanel.activeSelf)
        {
            GameManager.Instance.SetBlockEInput(true);
        }
        else
        {
            GameManager.Instance.SetBlockEInput(false);
        }
    }
    public void DeclareCulpable()
    {
        Debug.Log("Se declaró culpable a la imagen: " + clueImage.sprite.name);
        // Acá podés disparar un evento, cambiar estado del juego, etc.
    }
}
