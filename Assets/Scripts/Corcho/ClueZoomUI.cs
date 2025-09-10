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
    }

    public void ShowImage(Sprite sprite)
    {
        panel.SetActive(true);
        clueImage.sprite = sprite;
        clueImage.preserveAspect = true;
        clueInfo.text = "Imagen sin ID";
        culpableButton.gameObject.SetActive(true);
    }


    public void CloseClue()
    {
        panel.SetActive(false);
    }
    public void DeclareCulpable()
    {
        Debug.Log("Se declaró culpable a la imagen: " + clueImage.sprite.name);
        // Acá podés disparar un evento, cambiar estado del juego, etc.
    }
}
