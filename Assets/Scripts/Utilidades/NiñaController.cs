using UnityEngine;

public class NiñaController : MonoBehaviour
{
    [Header("Objeto que se enciende si ya conocés el nombre de Vanessa")]
    public GameObject Doll;

    private void Start()
    {
        if (Doll == null) return;

        // Seguridad: por si la escena arranca antes de que exista el clue tracker
        if (PlayerClueTracker.Instance == null)
        {
            Debug.LogWarning("NiñaController: PlayerClueTracker.Instance es null en Start().");
            Doll.SetActive(false);
            return;
        }

        bool hasClue = PlayerClueTracker.Instance.HasClue("Vanessa_name");
        Doll.SetActive(hasClue);
    }
}
