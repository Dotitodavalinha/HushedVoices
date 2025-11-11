using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ClueNotificationItem : MonoBehaviour
{
    [SerializeField] private Image _clueIcon;
    [SerializeField] private TMP_Text _clueText;
    [SerializeField] private CanvasGroup _canvasGroup;

    [SerializeField] private float _displayTime = 2.0f;
    [SerializeField] private float _fadeTime = 1.0f;

    public void Initialize(Sprite icon, string text)
    {
        _clueIcon.sprite = icon;
        _clueText.text = text;
        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy()
    {
        _canvasGroup.alpha = 1.0f;

        yield return new WaitForSeconds(_displayTime);

        float timer = 0f;
        while (timer < _fadeTime)
        {
            timer += Time.deltaTime;
            _canvasGroup.alpha = 1.0f - (timer / _fadeTime);
            yield return null;
        }

        Destroy(gameObject);
    }
}