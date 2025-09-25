using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorchoInteract : MonoBehaviour
{
    [SerializeField] public NOTEInteractionZone zonaInteraccion;
    [SerializeField] private ClueBoardManager corchoManager;
    [SerializeField] private FolioAnimation folio;


    [SerializeField] public GameObject PressE;
    [SerializeField] private bool UI_Activa = false;

    private void Start()
    {
        PressE.SetActive(false);

    }
    public void SetUIState(bool active)
    {
        UI_Activa = active;
    }
    private void Update()
    {

        if (zonaInteraccion.jugadorDentro)
        {
            if (!UI_Activa) PressE.SetActive(true);
            if (GameManager.Instance.BlockEInput)
                return;

            if (Input.GetKeyDown(KeyCode.E))
            {
                UI_Activa = !UI_Activa;

                if (UI_Activa)
                {
                    if (!GameManager.Instance.TryLockUI())
                    {
                        UI_Activa = false;
                        return;
                    }
                    PressE.SetActive(false);

                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    corchoManager.OpenBoard();
                    folio.OnUIReopened();
                    FindObjectOfType<ExitUnlocker>()?.MarcarCorchoUsado();
                }
                else
                {
                    GameManager.Instance.UnlockUI();
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    PressE.SetActive(true);
                    corchoManager.CloseBoard();
                }
            }
        }
        else
        {
            PressE.SetActive(false);
        }
    }


    public void ForceUIState(bool active)
    {
        UI_Activa = active;
    }

}
