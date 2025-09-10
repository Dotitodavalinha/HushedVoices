using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorchoInteract : MonoBehaviour
{
    [SerializeField] public NOTEInteractionZone zonaInteraccion;
    [SerializeField] private ClueBoardManager corchoManager;


    [SerializeField] public GameObject PressE;
    [SerializeField] private bool UI_Activa = false;

    private void Start()
    {
        PressE.SetActive(false);
    }

    private void Update()
    {
        if (zonaInteraccion.jugadorDentro)
        {
            if (!UI_Activa) PressE.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                UI_Activa = !UI_Activa;

                if (UI_Activa)
                {
                    if (!GameManager.Instance.TryLockUI())
                    {
                        UI_Activa = false; // cancela apertura
                        return;
                    }
                    PressE.SetActive(false);

                    //mostramos cursor y abrimos corcho
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    corchoManager.AbrirCorcho();

                    // Avisamos que el corcho fue usado
                    FindObjectOfType<ExitUnlocker>()?.MarcarCorchoUsado();
                }

                else
                {
                    GameManager.Instance.UnlockUI();
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    PressE.SetActive(true);
                    corchoManager.CerrarCorcho();
                }
            }
        }
        else
        {
            PressE.SetActive(false);
        }
    }


}
