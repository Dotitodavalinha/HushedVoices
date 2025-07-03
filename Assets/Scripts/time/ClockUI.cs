using UnityEngine;
using TMPro;

public class ClockUI : MonoBehaviour
{
    [SerializeField] private LightingManager lightingManager;
    [SerializeField] private TMP_Text clockText;

    private void Start()
    {
        lightingManager = FindObjectOfType<LightingManager>();
        clockText = GetComponent<TMP_Text>();
    }
    void Update()
    {
        float time = lightingManager.TimeOfDay;
        int hours = Mathf.FloorToInt(time);
        int minutes = Mathf.FloorToInt((time - hours) * 60);

        clockText.text = $"{hours:D2}:{minutes:D2}";
    }
}
