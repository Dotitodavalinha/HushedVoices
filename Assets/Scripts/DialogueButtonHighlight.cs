using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueButtonHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Outline outline;
    private bool isPointerOver = false;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
            outline.effectColor = Color.yellow; // Color del highlight
            outline.effectDistance = new Vector2(1.5f, 1.5f); // Grosor del outline
            outline.enabled = false;
        }
    }

    private void Update()
    {
        // Resaltar si es el botón actualmente seleccionado por EventSystem (teclado o mouse)
        if (EventSystem.current.currentSelectedGameObject == gameObject || isPointerOver)
        {
            outline.enabled = true;
        }
        else
        {
            outline.enabled = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;
        EventSystem.current.SetSelectedGameObject(gameObject); // Selecciona botón con mouse
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
    }
}
