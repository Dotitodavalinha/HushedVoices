using UnityEngine;

public class ClueInteractable : UIInteractable
{
    [Header("Configuración de pista")]
    [SerializeField] private string clueID;
    [SerializeField] private bool isImportant = false;

    protected override void OnActivate()
    {
        // Si ya la tiene, no mostramos nada
        if (PlayerClueTracker.Instance != null && PlayerClueTracker.Instance.HasClue(clueID))
        {
            Deactivate();
            return;
        }

        base.OnActivate();
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();

        if (!string.IsNullOrEmpty(clueID) && PlayerClueTracker.Instance != null)
        {
            PlayerClueTracker.Instance.AddClue(clueID);

            if (isImportant)
            {
                // Acá podés disparar efectos extra (sonido, alerta en pantalla, etc.)
                Debug.Log($"Se registró pista IMPORTANTE: {clueID}");
            }
            else
            {
                Debug.Log($"Se registró pista: {clueID}");
            }
        }
    }
}
