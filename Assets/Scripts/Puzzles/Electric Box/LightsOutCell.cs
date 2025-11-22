using UnityEngine;
using UnityEngine.UI;

public class LightsOutCell : MonoBehaviour
{
    public Sprite lightOnSprite;
    public Sprite lightOffSprite;

    private Image buttonImage;
    private bool isLightOn = true;
    private LightsOutGrid gridController;
    private int row;
    private int col;

    void Awake()
    {
        buttonImage = GetComponent<Image>();

        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnCellClicked);
        }

        UpdateAppearance();
    }

    public void Initialize(int r, int c, LightsOutGrid controller)
    {
        row = r;
        col = c;
        gridController = controller;
    }

    private void OnCellClicked()
    {
        gridController.CellClicked(row, col);
    }

    public void ToggleState()
    {
        isLightOn = !isLightOn;
        UpdateAppearance();
    }

    public void SetLightState(bool state)
    {
        isLightOn = state;
        UpdateAppearance();
    }

    private void UpdateAppearance()
    {
        if (buttonImage == null) return;
        buttonImage.sprite = isLightOn ? lightOnSprite : lightOffSprite;
    }

    public bool IsLightOn()
    {
        return isLightOn;
    }
}