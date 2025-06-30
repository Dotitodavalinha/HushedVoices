using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorchoInteract : MonoBehaviour
{
    [SerializeField] private PlayerMovementLocker playerLocker;
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
                    PressE.SetActive(false);
                    playerLocker.LockMovement();
                    corchoManager.AbrirCorcho(); // mostramos UI
                }
                else
                {
                    PressE.SetActive(true);
                    playerLocker.UnlockMovement();
                    corchoManager.CerrarCorcho(); // ocultamos UI
                }
            }

        }
        else
        {
            PressE.SetActive(false);
        }


    }

}
