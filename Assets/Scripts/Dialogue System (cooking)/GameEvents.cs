using UnityEngine;

public class GameEvents : MonoBehaviour
{
    [SerializeField] private PlayerResponseSO[] respuestas; // Asignas en el inspector
    [SerializeField] private GameObject ColegioStreet;

    void Awake()
    {
        foreach (var r in respuestas)
        {
            if (r.responseText.Contains("No decirle nada")) // o cualquier lógica para distinguir
            {
                r.onResponseChosen.RemoveAllListeners(); // evita duplicados en testing
                r.onResponseChosen.AddListener(() => ProgressManager.Instance.CambiarRootNPC("Chloe", "Root3"));
            }
            if (r.responseText.Contains("Decirle que no volvio a casa")) // o cualquier lógica para distinguir
            {
                r.onResponseChosen.RemoveAllListeners(); // evita duplicados en testing
                r.onResponseChosen.AddListener(() => ProgressManager.Instance.CambiarRootNPC("Chloe", "Root-4"));
            }

            
            if (r.responseText.Contains("Ok vuelvo luego")) 
            {
                r.onResponseChosen.RemoveAllListeners(); 
                r.onResponseChosen.AddListener(() => ProgressManager.Instance.CambiarRootNPC("Chloe", "Root"));
                r.onResponseChosen.AddListener(() => ProgressManager.Instance.Policez = true);
            }

            if (r.responseText.Contains("Muchas gracias")) 
            {
              //  r.onResponseChosen.RemoveAllListeners(); 
                r.onResponseChosen.AddListener(() => ProgressManager.Instance.CambiarRootNPC("PoliceZ", "RootPoliceZ1"));
                r.onResponseChosen.AddListener(() => ProgressManager.Instance.CambiarRootNPC("Chloe", "Root2"));
            }


            if (r.responseText.Contains("Si aca esta, para que puedas seguir trabajando"))
            {
                r.onResponseChosen.RemoveAllListeners(); 

                r.onResponseChosen.AddListener(() => ProgressManager.Instance.CambiarRootNPC("PoliceZ", "RootPoliceZ2"));
                r.onResponseChosen.AddListener(() => ProgressManager.Instance.Policeznt = true);
                r.onResponseChosen.AddListener(() => ProgressManager.Instance.ColegioStreet = true);
                r.onResponseChosen.AddListener(() => ColegioStreet.SetActive(ProgressManager.Instance.ColegioStreet));
            }
            if (r.responseText.Contains("¿Viste a Ben ultimamente?"))
            {
                r.onResponseChosen.RemoveAllListeners();
                r.onResponseChosen.AddListener(() => ProgressManager.Instance.CambiarRootNPC("Marina", "RootMarina1"));
            }
         


        }
    }
}
