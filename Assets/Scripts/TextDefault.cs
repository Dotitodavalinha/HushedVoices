using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TextDefault : MonoBehaviour
{
    [SerializeField] public NPCInteractionZone zonaInteraccion;
    [SerializeField] public TMP_Text text;

    private void Start()
    {
        text.gameObject.SetActive(true);
        text.text = "";
    }

    private void Update()
    {
        if (zonaInteraccion.jugadorDentro)
        {
            text.text = "Press 'E' to talk";
        }
        else
        {
            text.text = "";
        }
    }
}