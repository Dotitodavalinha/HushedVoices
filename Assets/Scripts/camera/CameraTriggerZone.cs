using UnityEngine;
using System.Collections;

public class CameraTriggerZone : MonoBehaviour
{
    public CameraManagerZ manager;
    public Cinemachine.CinemachineVirtualCamera targetCamera;
    public Material Static;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (manager.GetCurrentCamera() == targetCamera) return;

        Static.SetInt("_Turn", 1);
        StartCoroutine(Wait());

        manager.SwitchCamera(targetCamera);
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.1f);
        Static.SetInt("_Turn", 0);
    }
}

