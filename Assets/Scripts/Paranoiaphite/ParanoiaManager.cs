using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ParanoiaManager : MonoBehaviour
{
    public static ParanoiaManager Instance { get; private set; }

    [Range(0f, 1f)]
    public float paranoiaLevel = 0f;
    private ParanoiaObject[] paranoiaObjects;
   

    [Header("Shaders/Materials")]
    public Material vignette;
    public Material dayNightShader;
    public Material cameraLines;

    [Header("UI")]
    public ojoParanoia ojito;
    [SerializeField] private TextMeshProUGUI paranoiaName;
    [SerializeField] private TextMeshProUGUI paranoiaText;
    private Color colorNormal = new Color32(104, 38, 25, 255);
    private Color colorParanoia = Color.white;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        paranoiaObjects = FindObjectsOfType<ParanoiaObject>();
        ojito=FindObjectOfType<ojoParanoia>();

        GameObject postProcessingObj = GameObject.Find("PostProcessing");
        if (postProcessingObj != null)
        {
            Renderer renderer = postProcessingObj.GetComponent<Renderer>();
            if (renderer != null)
            {
                vignette = renderer.sharedMaterial;
                dayNightShader = renderer.sharedMaterial;
                cameraLines = renderer.sharedMaterial;
            }
        }

        ApplyParanoia();
    }

    public void ResetManager()
    {
        paranoiaLevel = 0f;
        ApplyParanoia();
    }

    public void ModifyParanoia(float delta)
    {
        paranoiaLevel = Mathf.Clamp01(paranoiaLevel + delta);
        ApplyParanoia();
    }

    public void SetParanoiaValueDirect(float value)
    {
        paranoiaLevel = Mathf.Clamp01(value);
        ApplyParanoia();
    }

    private void ApplyParanoia()
    {
        // Shaders
        if (vignette != null) vignette.SetFloat("_vig_amount", paranoiaLevel * 0.5f);
        if (dayNightShader != null) dayNightShader.SetFloat("dayNight", paranoiaLevel);
        if (cameraLines != null) cameraLines.SetFloat("_Paranoia", paranoiaLevel);

        // UI
        if (paranoiaText != null && paranoiaName != null)
        {
            Color colorInterpolado = Color.Lerp(colorNormal, colorParanoia, paranoiaLevel);
            paranoiaText.color = colorInterpolado;
            paranoiaName.color = colorInterpolado;
        }

        // Objetos
        if (paranoiaObjects != null)
        {
            foreach (var obj in paranoiaObjects)
                obj.SetParanoia(paranoiaLevel);
        }

        if (ojito != null)
            ojito.setSprite(paranoiaLevel);

        // DialogueManager botones
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.SetModoParanoia(paranoiaLevel >= 1f);
    }

    public void RegisterParanoiaObject(ParanoiaObject obj)
    {
        obj?.SetParanoia(paranoiaLevel);
    }
}