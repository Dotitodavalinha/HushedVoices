using UnityEngine;

public class ClueInteractable : UIInteractable
{
    [Header("Configuración de pista")]
    [SerializeField] private string clueID;
    [SerializeField] private bool isImportant_ShowAlert = false;
    private void Awake()
    {
        if (destroyAfterUse == true)
        {
            if (PlayerClueTracker.Instance != null && PlayerClueTracker.Instance.HasClue(clueID))  // Si ya la tiene, no mostramos nada
            {
                Deactivate();
                return;
            }
        }
    }
    protected override void OnActivate()
    {
        if (destroyAfterUse == true)
        {
            if (PlayerClueTracker.Instance != null && PlayerClueTracker.Instance.HasClue(clueID))  // Si ya la tiene, no mostramos nada
            {
                Deactivate();
                return;
            }
        }


        base.OnActivate();
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        SoundManager.instance.PlaySound(SoundID.CluePickupSound);

        if (!string.IsNullOrEmpty(clueID) && PlayerClueTracker.Instance != null)
        {
            PlayerClueTracker.Instance.AddClue(clueID);

            if (isImportant_ShowAlert)
            {
                // alerta en pantalla
                Debug.Log($"Se registró pista IMPORTANTE: {clueID}");
            }
            else
            {
                Debug.Log($"Se registró pista: {clueID}");
            }
        }
    }
}
