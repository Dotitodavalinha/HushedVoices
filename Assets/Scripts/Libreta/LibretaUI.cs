using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibretaUI : MonoBehaviour
{
    [SerializeField] private PlayerMovementLocker playerLocker;
    [SerializeField] private TabsLogic tabsLogic;

    [SerializeField] private GameObject notaBenUI;
    [SerializeField] private GameObject cafeteriaUI;
    [SerializeField] private GameObject PoliciazUI;
    [SerializeField] private GameObject PolicezntUI;
    [SerializeField] private GameObject GotCoffe;
    [SerializeField] private GameObject LostCoffe;

    private bool libretaAbierta = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            libretaAbierta = !libretaAbierta;

            if (libretaAbierta)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                ActualizarUI();
                playerLocker.LockMovement();

                tabsLogic.OpenSection(0); // ← activa index 0
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                playerLocker.UnlockMovement();

                tabsLogic.CloseAllSections(); // ← desactiva todas las pestañas
            }
        }
    }

    public void AbrirLibretaDesdeBoton()
    {
        if (!libretaAbierta)
        {
            libretaAbierta = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            ActualizarUI();
            playerLocker.LockMovement();
        }
    }

    void ActualizarUI()
    {
        notaBenUI.SetActive(ProgressManager.Instance.BensNoteUnlocked);
        cafeteriaUI.SetActive(ProgressManager.Instance.CoffeeShopUnlocked);
        PoliciazUI.SetActive(ProgressManager.Instance.Policez);
        PolicezntUI.SetActive(ProgressManager.Instance.Policeznt);
        GotCoffe.SetActive(ProgressManager.Instance.GotCoffe);
        LostCoffe.SetActive(ProgressManager.Instance.LostCoffe);
    }
}
