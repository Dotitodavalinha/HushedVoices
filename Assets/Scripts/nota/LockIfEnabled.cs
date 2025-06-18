using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockIfEnabled : MonoBehaviour
{
    [SerializeField] private PlayerMovementLocker playerLocker;
    private void OnEnable() 
    {
       playerLocker.LockMovement();
    }
    private void Update()
    {
        playerLocker.LockMovement();
    }
    private void OnDisable()
    {
        playerLocker.UnlockMovement();
    }
}
