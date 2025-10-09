using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class ClueAlertFade : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI clueText; 
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float displayTime;

    void Start()
    {
        StartCoroutine(PlayFade());
    }

    private IEnumerator PlayFade()
    {
        Color imgColor = backgroundImage.color;
        Color textColor = clueText.color;

        // Fade In
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            backgroundImage.color = new Color(imgColor.r, imgColor.g, imgColor.b, a);
            clueText.color = new Color(textColor.r, textColor.g, textColor.b, a);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(displayTime);

        // Fade Out
        t = 0f;
        while (t < fadeDuration)
        {
           t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(1f, 0f, t / fadeDuration);
            backgroundImage.color = new Color(imgColor.r, imgColor.g, imgColor.b, a);
            clueText.color = new Color(textColor.r, textColor.g, textColor.b, a);
            yield return null;
        }

        Destroy(gameObject);
    }
}
