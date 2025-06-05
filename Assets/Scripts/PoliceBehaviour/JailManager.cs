using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JailManager : MonoBehaviour
{
    public static JailManager Instance;

    public int counter = 0;
    public int maxValue = 3;
    public GameObject objectToActivate;

    private bool triggered = false;

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
        Debug.Log("JailManager persistente creado");
    }

    private void Start()
    {
        objectToActivate.SetActive(false);
    }
    public void Increment()
    {
        Debug.Log("Sumando uno al contador de la carcel!");
        Debug.Log("Contador: " + counter);
        if (triggered) return;

        counter++;
        if (counter >= maxValue)
        {
            triggered = true;

            if (objectToActivate != null)
            {
                objectToActivate.SetActive(true);
                counter = 0;
            }

            Time.timeScale = 0f; // Pausa total
            StartCoroutine(WaitAndGoToRoom());
        }
    }

    private IEnumerator WaitAndGoToRoom()
    {
        yield return new WaitForSecondsRealtime(3f);

        Time.timeScale = 1f; // Reanuda por si se usa en otra escena
        if (GameManager.Instance == null)
        {
            GameObject gm = new GameObject("GameManager");
            gm.AddComponent<GameManager>();
        }

        GameManager.Instance.LoadScene("Room");
        objectToActivate.SetActive(false);
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
        objectToActivate = GameObject.Find("CaisteEnCana"); 
        if (objectToActivate != null)
            objectToActivate.SetActive(false);
    }

}
