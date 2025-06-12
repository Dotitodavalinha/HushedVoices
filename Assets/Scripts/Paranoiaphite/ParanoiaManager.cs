using UnityEngine;

public class ParanoiaManager : MonoBehaviour
{
    [SerializeField] private float paranoiaLevel = 0f;
    private ParanoiaObject[] paranoiaObjects;
    public Material vignette;
    public Material cameraLines;

    private void Awake()
    {
        paranoiaObjects = FindObjectsOfType<ParanoiaObject>();
    }

    public void SetParanoia(float value)
    {
        Debug.LogWarning("Paranoia actualizada a " + value);
        paranoiaLevel = Mathf.Clamp01(value);

        foreach (var obj in paranoiaObjects)
        {
            obj.SetParanoia(paranoiaLevel);
        }
    }

    void Update() //test
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            SetParanoia(0f);
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
