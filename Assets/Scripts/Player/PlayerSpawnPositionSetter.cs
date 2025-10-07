using System.Collections;
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

        string entryPoint = SceneTransitionManager.Instance != null ? SceneTransitionManager.Instance.lastEntryPointID : null;
        if (!string.IsNullOrEmpty(entryPoint))
        {
            Transform spawn = GameObject.Find(entryPoint)?.transform;
            if (spawn != null)
            {
                transform.position = spawn.position;
                transform.rotation = spawn.rotation;
                Debug.LogWarning("Se movió al player al SpawnPoint " + entryPoint);

                // resetear CharacterController para evitar que quede "atascado" por colisiones
                var controller = GetComponent<CharacterController>();
                if (controller != null)
                {
                    controller.enabled = false;
                    // esperar 1 frame para asegurar que el motor procese la desactivación
                    yield return null;
                    controller.enabled = true;
                }

                // opcional: resetear animator speed param
                var anim = GetComponent<Animator>();
                if (anim != null) anim.SetFloat("Speed", 0f);

                // limpiar entryPoint para que no reaplique el mismo spawn
                SceneTransitionManager.Instance.lastEntryPointID = null;
            }
            else
            {
                Debug.LogWarning("No se encontró el punto de entrada: " + entryPoint);
            }
        }
    }
}
