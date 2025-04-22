using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TextDefault : MonoBehaviour
{
    [SerializeField] public NPCInteractionZone zonaInteraccion;
    [SerializeField] public TMP_Text text;

    private bool yaInteractuó = false;

    private void Start()
    {
        text.gameObject.SetActive(true);
        text.text = "";
    }

    private void Update()
    {
        if (zonaInteraccion.jugadorDentro && !yaInteractuó)
        {
            text.text = "Press 'E' to talk";

            if (Input.GetKeyDown(KeyCode.E))
            {
                yaInteractuó = true;
                text.text = "";
            }
        }
        else if (!zonaInteraccion.jugadorDentro)
        {
            yaInteractuó = false;
            text.text = "";
        }
    }
}