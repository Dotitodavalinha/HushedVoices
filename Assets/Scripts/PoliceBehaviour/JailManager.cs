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
            Debug.LogWarning("a la carcelphitee");

            Debug.Log("ARRESTO: Intentando llamar a PlayerClueTracker.LoseAllClues()");
            if (PlayerClueTracker.Instance != null)
            {
                PlayerClueTracker.Instance.LoseAllClues();
            }
            else
            {
                Debug.LogError("ARRESTO FALLIDO: PlayerClueTracker.Instance es NULO.");
            }

            if (objectToActivate != null)
            {
                objectToActivate.SetActive(true);
                counter = 0;
            }
            else
            {
                Debug.Log("no se encontro el png de 'fuiste encarcelado'");
            }

            StartCoroutine(WaitAndGoToRoom());
        }
    }
    public void SetMaxValue()
    {
        if (CheatInmortal)
        {
            return;
        }
        else
        {
            Debug.LogWarning("a la carcelphitee");

            Debug.Log("ARRESTO: Intentando llamar a PlayerClueTracker.LoseAllClues()");
            if (PlayerClueTracker.Instance != null)
            {
                PlayerClueTracker.Instance.LoseAllClues();
            }
            else
            {
                Debug.LogError("ARRESTO FALLIDO: PlayerClueTracker.Instance es NULO.");
            }


            if (objectToActivate != null)
            {
                objectToActivate.SetActive(true);
                counter = 0;
            }
            else
            {
                Debug.LogWarning("no se encontro el png de 'fuiste encarcelado'");
            }

            StartCoroutine(WaitAndGoToRoom());
            Debug.Log("go to room corrutina");
        }
    }
    private IEnumerator WaitAndGoToRoom()
    {
        Debug.Log("va a esperar 3 segundos");
        yield return new WaitForSecondsRealtime(3f);
        Debug.Log("ya pasaron 3 segundos");

        SceneTransitionData data = new SceneTransitionData("StationInside", "DesdeJail");
        SceneTransitionManager.Instance.TransitionToScene(data);
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