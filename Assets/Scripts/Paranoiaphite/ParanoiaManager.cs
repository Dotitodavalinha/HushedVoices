using UnityEngine;
using UnityEngine.SceneManagement;

public class ParanoiaManager : MonoBehaviour
{
    public static ParanoiaManager Instance { get; private set; }

    [SerializeField] private float paranoiaLevel = 0f;
    private ParanoiaObject[] paranoiaObjects;
    public Material vignette;
    public Material cameraLines;

    private void Awake()
    {
        // Singleton Pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        paranoiaObjects = FindObjectsOfType<ParanoiaObject>();

        // Suscribirse al evento de cambio de escena
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Evita fugas de memoria al desuscribirse
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        paranoiaObjects = FindObjectsOfType<ParanoiaObject>();
        SetParanoia(paranoiaLevel); 
    }

    public void SetParanoia(float value)
    {
        Debug.LogWarning("Paranoia actualizada " + value);
        paranoiaLevel = Mathf.Clamp01(paranoiaLevel + value);

        foreach (var obj in paranoiaObjects)
        {
            obj.SetParanoia(paranoiaLevel);
        }

        if (DialogueManager.Instance != null)
            DialogueManager.Instance.SetModoParanoia(paranoiaLevel >= 1f); //entrara como true cuando la paranioa este al max

    }

    void Update() // test
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            SetParanoia(-1f);
            vignette.SetFloat("_vig_amount", 0f);
            cameraLines.SetFloat("_scanningLinesAmount", 0f);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            SetParanoia(1f);
            vignette.SetFloat("_vig_amount", 0.33f);
            cameraLines.SetFloat("_scanningLinesAmount", 0.1f);
        }
    }
}
