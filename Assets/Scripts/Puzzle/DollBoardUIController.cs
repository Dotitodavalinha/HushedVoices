using UnityEngine;
using System;
using System.Linq;

public class DollBoardUIController : MonoBehaviour
{
    [Serializable]                 
    public struct PartEntry
    {
        public string id;
        public GameObject partGO;
    }

    [Header("Parts on board")]
    [SerializeField] GameObject dollWhole;   // off por defecto
    [SerializeField] PartEntry[] entries;    // todas off por defecto

    int revealed;

    void Awake()
    {
        if (dollWhole) dollWhole.SetActive(false);
        if (entries != null)
            foreach (var e in entries) if (e.partGO) e.partGO.SetActive(false);
        revealed = 0;
    }

    public void RevealPart(string id)
    {
        var e = entries.FirstOrDefault(x => x.id == id);
        if (e.partGO && !e.partGO.activeSelf)
        {
            e.partGO.SetActive(true);
            revealed++;
        }
    }

    public bool IsComplete(int totalNeeded) => revealed >= totalNeeded;

    public void PlayCompleteAndClose(Action onDone)
    {
        // apaga “divididas”
        if (entries != null)
            foreach (var e in entries) if (e.partGO) e.partGO.SetActive(false);

        // prende muñeca entera + pop suave (con o sin LeanTween)
        if (dollWhole)
        {
            dollWhole.SetActive(true);
            var t = dollWhole.transform;
            t.localScale = Vector3.one * 0.9f;

#if LEANTWEEN      // define este símbolo si usás LeanTween
            LeanTween.scale(t.gameObject, Vector3.one, 0.25f)
                     .setEaseOutBack()
                     .setOnComplete(() => onDone?.Invoke());
#else
            // fallback simple sin dependencias
            t.localScale = Vector3.one;
            onDone?.Invoke();
#endif
        }
        else onDone?.Invoke();
    }
}
