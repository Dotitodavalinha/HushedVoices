using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlinersController : MonoBehaviour //durante la noche activo ciertos Ouliners para marcar objetos.
{

    [SerializeField] private LightingManager timeManager;
    [SerializeField] private List<GameObject> objectsToActivate = new List<GameObject>();

    private void Update()
    {
        if(timeManager != null)
        {
            foreach (GameObject obj in objectsToActivate)
            {
                if (obj != null)
                    obj.SetActive(timeManager.IsNight); // si es de noche se activan
            }
        }
        else
        {
            Debug.LogWarning("No hay ref al timeManager");
        }
        
    }
}
