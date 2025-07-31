
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    [SerializeField] private PlayerResponseSO[] respuestas; // Asignas en el inspector
    [SerializeField] private GameObject ColegioStreet;

    void Awake()
    {
        foreach (var r in respuestas)
        {
            if (r.responseText.Contains("No, nothing")) // o cualquier lógica para distinguir
            {
                r.onResponseChosen.RemoveAllListeners(); // evita duplicados en testing
                r.onResponseChosen.AddListener(() => ProgressManager.Instance.CambiarRootNPC("Chloe", "Root3"));
            }
            if (r.responseText.Contains("He didn't come home yesterday")) // o cualquier lógica para distinguir
            {
                r.onResponseChosen.RemoveAllListeners(); // evita duplicados en testing
                r.onResponseChosen.AddListener(() => ProgressManager.Instance.CambiarRootNPC("Chloe", "Root-4"));
            }

            
            if (r.responseText.Contains("Ok, what if I bring you a coffee?")) 
            {
                r.onResponseChosen.RemoveAllListeners(); 
                r.onResponseChosen.AddListener(() => ProgressManager.Instance.CambiarRootNPC("Chloe", "Root"));
                r.onResponseChosen.AddListener(() => ProgressManager.Instance.Policez = true);
            }

            if (r.responseText.Contains("Thanks Chloe")) 
            {
                //r.onResponseChosen.RemoveAllListeners(); 
                r.onResponseChosen.AddListener(() => ProgressManager.Instance.CambiarRootNPC("PoliceZ", "RootPoliceZ1"));
                r.onResponseChosen.AddListener(() => ProgressManager.Instance.CambiarRootNPC("Chloe", "Root2"));
                r.onResponseChosen.AddListener(() => ProgressManager.Instance.GotCoffe = true);
            }


            if (r.responseText.Contains("Yeah, here you go sir"))
            {
                r.onResponseChosen.RemoveAllListeners(); 

                r.onResponseChosen.AddListener(() => ProgressManager.Instance.CambiarRootNPC("PoliceZ", "RootPoliceZ2"));
                r.onResponseChosen.AddListener(() => ProgressManager.Instance.Policeznt = true);
                r.onResponseChosen.AddListener(() => ProgressManager.Instance.ColegioStreet = true);
                r.onResponseChosen.AddListener(() => ColegioStreet.SetActive(ProgressManager.Instance.ColegioStreet));
                r.onResponseChosen.AddListener(() => ColegioStreet.SetActive(false)) ;
                r.onResponseChosen.AddListener(() => ProgressManager.Instance.LostCoffe = true);
                r.onResponseChosen.AddListener(() => ProgressManager.Instance.GotCoffe = false);
            }
            if (r.responseText.Contains("Have you seen Ben?"))
            {
                r.onResponseChosen.RemoveAllListeners();
                r.onResponseChosen.AddListener(() => ProgressManager.Instance.CambiarRootNPC("Marina", "RootMarina1"));
            }

            if(r.responseText.Contains("..."))
            {
                /*
                r.onResponseChosen.RemoveAllListeners();
                r.onResponseChosen.AddListener(() => ProgressManager.Instance.CambiarRootNPC("Police", "RootPolice1"));
                r.onResponseChosen.AddListener(() => {
                    JailManager jail = FindObjectOfType<JailManager>();
                    if (jail != null)
                    {
                        jail.SetMaxValue();
                    }
                    else
                    {
                        Debug.LogWarning("No se encontró JailManager en la escena.");
                    }
                });
                */

            }

           
            if (r.responseText.Contains("Vanessa"))
            {
                // r.onResponseChosen.RemoveAllListeners();

                r.onResponseChosen.AddListener(() => PlayerClueTracker.Instance.AddClue("parkGuy"));
                Debug.Log("cojeme fede");

            } 
             
            

        }
    }
}
