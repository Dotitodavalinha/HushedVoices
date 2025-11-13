
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents Instance { get; private set; }

    [SerializeField] private PlayerResponseSO[] respuestas;
    [SerializeField] private GameObject ColegioStreet;

    // evita configurar dos veces el mismo SO cuando hay múltiples GameEvents
    private readonly HashSet<PlayerResponseSO> _registered = new HashSet<PlayerResponseSO>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // fusiona respuestas de esta escena en el singleton existente
            foreach (var r in respuestas)
                Instance.TryRegister(r);

            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (var r in respuestas)
            TryRegister(r);
    }

    public void TryRegister(PlayerResponseSO r)
    {
        if (r == null || _registered.Contains(r)) return;
        _registered.Add(r);
        WireUp(r); // configura sus listeners una sola vez
    }

    private void WireUp(PlayerResponseSO r)
    {
        if (r.responseText.Contains("No, nothing"))
        {
            r.onResponseChosen.RemoveAllListeners();
            r.onResponseChosen.AddListener(() => ProgressManager.Instance.CambiarRootNPC("Chloe", "Root3"));
        }

        if (r.responseText.Contains("He didn't come home yesterday"))
        {
            r.onResponseChosen.RemoveAllListeners();
            r.onResponseChosen.AddListener(() => ProgressManager.Instance.CambiarRootNPC("Chloe", "Root-4"));
        }

        if (r.responseText.Contains("Ok, what if I bring you a coffee?"))
        {
            Debug.LogWarning("Wiring up coffee response");
            r.onResponseChosen.RemoveAllListeners();
            r.onResponseChosen.AddListener(() => ProgressManager.Instance.CambiarRootNPC("Chloe", "Root"));
            r.onResponseChosen.AddListener(() => ProgressManager.Instance.Policez = true);
        }

        if (r.responseText.Contains("Hi, can you make me a black coffee please?"))
        {
            // r.onResponseChosen.RemoveAllListeners();
            r.onResponseChosen.AddListener(() => ProgressManager.Instance.CambiarRootNPC("PolicemanZ", "RootPoliceZ1"));
            r.onResponseChosen.AddListener(() => ProgressManager.Instance.CambiarRootNPC("Chloe", "Root2"));
            r.onResponseChosen.AddListener(() => ProgressManager.Instance.GotCoffe = true);
        }

        if (r.responseText.Contains("Yeah, here you go sir"))
        {
            r.onResponseChosen.RemoveAllListeners();

            r.onResponseChosen.AddListener(() => ProgressManager.Instance.CambiarRootNPC("PolicemanZ", "RootPoliceZ2"));
            r.onResponseChosen.AddListener(() => ProgressManager.Instance.Policeznt = true);
            r.onResponseChosen.AddListener(() => ProgressManager.Instance.ColegioStreet = true);
            r.onResponseChosen.AddListener(() => ProgressManager.Instance.PolicemanZDialogueRoot = "RootPoliceZ2");

            //  cambio: null-safe entre escenas (si la referencia no existe en esta escena, busca por nombre)
            r.onResponseChosen.AddListener(() =>
            {
                if (ColegioStreet != null) ColegioStreet.SetActive(false);
                else
                {
                    var go = GameObject.Find("ColegioStreet");
                    if (go != null) go.SetActive(false);
                }
            });

            r.onResponseChosen.AddListener(() => ProgressManager.Instance.LostCoffe = true);
            r.onResponseChosen.AddListener(() => ProgressManager.Instance.GotCoffe = false);
        }

        if (r.responseText.Contains("Have you seen Ben?"))
        {
            r.onResponseChosen.RemoveAllListeners();
            r.onResponseChosen.AddListener(() => ProgressManager.Instance.CambiarRootNPC("Marina", "RootMarina1"));
        }
        if (r.responseText.Contains("Let me see..."))
        {
            Debug.LogWarning("Wiring up girl drawing response");
            // NO borro los otros listeners: dejo que el sistema de diálogo cierre la UI como siempre
            r.onResponseChosen.AddListener(() =>
            {
                Debug.LogWarning("Recibo dibujo de la niña");
                if (PuzzleManager.Instance != null)
                    PuzzleManager.Instance.ShowDollDrawing();   // nuevo método
            });
        }


        if (r.responseText.Contains("Vanessa"))
        {
            // r.onResponseChosen.RemoveAllListeners();
            r.onResponseChosen.AddListener(() => PlayerClueTracker.Instance.AddClue("parkGuy"));
        }

        if (string.Equals(r.name, "no ah vuelto a casa...", System.StringComparison.OrdinalIgnoreCase))
        {
            r.onResponseChosen.RemoveAllListeners();
            r.onResponseChosen.AddListener(() => ProgressManager.Instance.CambiarRootNPC("Marina", "RootMarina1"));
        }
    }
}



