using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibretaUI : MonoBehaviour
{
    [SerializeField] private PlayerMovementLocker playerLocker;

    [SerializeField] private GameObject libretaCanvas;
    [SerializeField] private GameObject notaBenUI;
    [SerializeField] private GameObject cafeteriaUI;

    private bool libretaAbierta = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            libretaAbierta = !libretaAbierta;
            libretaCanvas.SetActive(libretaAbierta);

            if (libretaAbierta)
            {
                ActualizarUI();
                playerLocker.LockMovement();
            }
            else
            {
                playerLocker.UnlockMovement();
            }
        }
    }

    void ActualizarUI()
    {
        notaBenUI.SetActive(ProgressManager.Instance.BensNoteUnlocked);
        cafeteriaUI.SetActive(ProgressManager.Instance.CoffeeShopUnlocked);
    }
}

