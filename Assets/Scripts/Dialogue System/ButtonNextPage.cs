using UnityEngine;
using UnityEngine.UI;

public class ButtonNextPage : MonoBehaviour
{
    void Start()
    {
        Button miBoton = GetComponent<Button>();
        miBoton.onClick.AddListener(() => DialogueManager.Instance.OnNextPageButtonPressed());
    }
}
