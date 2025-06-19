using UnityEngine;
using UnityEngine.SceneManagement;

public class ParanoiaManager : MonoBehaviour
{
    public static ParanoiaManager Instance { get; private set; }

    [SerializeField] private float paranoiaLevel = 0f;
    private ParanoiaObject[] paranoiaObjects;
    public Material vignette;
    public Material dayNightShader;

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

        paranoiaLevel = 0f;
        vignette.SetFloat("_vig_amount", paranoiaLevel);
        dayNightShader.SetFloat("dayNight", paranoiaLevel);

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
        //SetParanoiaValue(0);
        
    }

    public void SetParanoiaValue(float value)
    {
        Debug.LogWarning("Paranoia actualizada " + value);
        paranoiaLevel = Mathf.Clamp01(paranoiaLevel + value);

        foreach (var obj in paranoiaObjects)
        {
            obj.SetParanoia(paranoiaLevel);
        }

        vignette.SetFloat("_vig_amount", paranoiaLevel);
        dayNightShader.SetFloat("dayNight", paranoiaLevel);



        if (DialogueManager.Instance != null)
            DialogueManager.Instance.SetModoParanoia(paranoiaLevel >= 1f); //entrara como true cuando la paranioa este al max

    }

    public void RegisterParanoiaObject(ParanoiaObject obj)
    {
        obj.SetParanoia(paranoiaLevel);
    }

        void Update() // test
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            SetParanoiaValue(-1f);

        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            SetParanoiaValue(1f);
        }
    }
}
