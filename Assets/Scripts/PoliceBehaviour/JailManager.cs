using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JailManager : MonoBehaviour
{
    public static JailManager Instance;

    public int counter = 0;
    public int maxValue = 4;
    public GameObject objectToActivate;

    private bool triggered = false;

    [SerializeField] private bool CheatInmortal;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("JailManager duplicado, destruyendo...");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        //Debug.Log("JailManager persistente creado");
    }

    private void Start()
    {
        //objectToActivate.SetActive(false);
    }
    public void Increment()
    {
        Debug.Log("Sumando uno al contador de la carcel!");
        Debug.Log("Contador: " + counter);
        if (triggered) return;

        counter++;
        if (counter >= maxValue)
        {
            //triggered = true;
            Debug.Log("a la carcelphitee");
            if (objectToActivate != null)
            {
                objectToActivate.SetActive(true);
                counter = 0;
            }
            else
            {
                Debug.Log("no se encontro el png de 'fuiste encarcelado'");
            }

            // Time.timeScale = 0f; // Pausa total
            StartCoroutine(WaitAndGoToRoom());
        }
    }
    public void SetMaxValue()
    {
        if (CheatInmortal) // si activamos un bool no podemos caer presos, basicamente. Es para debug.
        {
            return;
        }
        else
        {
            // triggered = true;
            Debug.Log("a la carcelphitee");
            if (objectToActivate != null)
            {
                objectToActivate.SetActive(true);
                counter = 0;
            }
            else
            {
                Debug.Log("no se encontro el png de 'fuiste encarcelado'");
            }

            // Time.timeScale = 0f; // Pausa total
            StartCoroutine(WaitAndGoToRoom());
            Debug.Log("go to room corrutina");
        }
    }
    private IEnumerator WaitAndGoToRoom()
    {
        Debug.Log("va a esperar 3 segundos");
        yield return new WaitForSecondsRealtime(3f);
        //triggered = false;
        Debug.Log("ya pasaron 3 segundos");
        // Time.timeScale = 1f; // Reanuda por si se usa en otra escena
        var resetter = FindObjectOfType<FinalDayBackToMenu>();
        if (resetter != null)
        {
          
            resetter.ResetAllManagers();
            ProgressManager.Instance.CambiarRootNPC("PolicemanZ", "RootPoliceZ0");
            Debug.Log("Se llamo a ResetAllManagers desde JailManager");
        }
        else
        {
            Debug.LogWarning("No se encontro FinalDayBackToMenu en la escena!");
        }

        GameManager.Instance.LoadScene("Room");
        // Usa el nuevo método directo para resetear a 0
        ParanoiaManager.Instance.SetParanoiaValueDirect(0f);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;
        Transform container = GameObject.Find("CarcelContainer")?.transform;
        if (container != null)
        {
            objectToActivate = container.Find("CaisteEnCana")?.gameObject;
            if (objectToActivate != null)
                objectToActivate.SetActive(false);
            else
                Debug.LogWarning("No se encontró el PNG hijo");
        }
        else
        {
            Debug.LogWarning("No se encontró el contenedor CarcelContainer");
        }
    }
}