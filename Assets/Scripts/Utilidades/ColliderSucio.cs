using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CleanItem;

public class ColliderSucio : MonoBehaviour
{
    [SerializeField] public GameObject Dialogo;
    private GameObject Player;
    PlayerMovementLocker playerMovementLocker;



    private void Start()
    {
        Player = GameObject.FindWithTag("Player");
        playerMovementLocker = Player.GetComponent<PlayerMovementLocker>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // solo el player lo activa
        {
            if (!GameManager.Instance.TryLockUI())
                return;
            if (playerMovementLocker != null) playerMovementLocker.LockMovement();
            Dialogo.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                Dialogo.SetActive(false);

                GameManager.Instance.UnlockUI();
                Destroy(this);

            }
        }



    }

}
