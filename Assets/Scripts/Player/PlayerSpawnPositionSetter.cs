using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPositionSetter : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(SetSpawnPositionNextFrame());
    }

    IEnumerator SetSpawnPositionNextFrame()
    {
        yield return null; // esperar 1 frame. sin esto a veces simplemente no funciona el sistema, xd

        string entryPoint = SceneTransitionManager.Instance.lastEntryPointID;
        if (!string.IsNullOrEmpty(entryPoint))
        {
            Transform spawn = GameObject.Find(entryPoint)?.transform;
            if (spawn != null)
            {
                transform.position = spawn.position;
                transform.rotation = spawn.rotation;
                Debug.LogWarning("Se movió al player al SpawnPoint " + entryPoint);
            }
            else
            {
                Debug.LogWarning("No se encontró el punto de entrada: " + entryPoint);
            }
        }
    }
}
