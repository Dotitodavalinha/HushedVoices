using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressKeyTodestroyObject : MonoBehaviour
{
    public GameObject ObjectToActivate;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            ObjectToActivate.SetActive(true);
        }
    }
}
