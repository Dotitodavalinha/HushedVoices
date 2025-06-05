using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockChloe2 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        ProgressManager.Instance.Policez = false;
        ProgressManager.Instance.CambiarRootNPC("PoliceZ", "RootPoliceZ1");
        ProgressManager.Instance.Policeznt = true;

    }
}

