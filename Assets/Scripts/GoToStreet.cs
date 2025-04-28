using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToStreet : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager.Instance.LoadScene("Street");

    }
}
