using UnityEngine;
using UnityEngine.SceneManagement;


public class ImportantClue : MonoBehaviour
{
    public static ImportantClue Instance;

    [SerializeField] private GameObject clueAlertPrefab;
    [SerializeField] private Transform canvasTransform;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        AssignCanvas();
    }


    public void ShowClueAlert()
    {
        Debug.LogWarning("ALERTA PISTA IMPORTANTE");
        GameObject alert = Instantiate(clueAlertPrefab, canvasTransform);
        alert.transform.SetAsLastSibling();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignCanvas();
    }

    private void AssignCanvas()
    {
        var canvasObj = GameObject.Find("Canvas");
        if (canvasObj != null)
        {
            canvasTransform = canvasObj.transform;
        }
        else
        {
            Debug.LogWarning("No se encontró un objeto llamado 'Canvas' en la escena.");
        }
    }

}

