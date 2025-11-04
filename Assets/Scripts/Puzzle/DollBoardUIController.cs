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

    [SerializeField] private float holdSeconds;   // cuánto tiempo queda abierto
    [SerializeField, Range(1f, 2f)] private float endScale; // escala final de la Whole

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

    // REEMPLAZO COMPLETO
    public void PlayCompleteAndClose(System.Action onDone)
    {
        StartCoroutine(Co_PlayCompleteAndClose(onDone));
    }

    private System.Collections.IEnumerator Co_PlayCompleteAndClose(System.Action onDone)
    {
        // 1) Apaga partes "divididas"
        if (entries != null)
            foreach (var e in entries) if (e.partGO) e.partGO.SetActive(false);

        // 2) Prende Whole Doll
        if (dollWhole)
        {
            dollWhole.SetActive(true);

            var t = dollWhole.transform;
            // Estado inicial: escala 1 (o lo que tengas en prefab)
            Vector3 start = t.localScale;
            Vector3 target = Vector3.one * endScale;

            // 3) Mantener visible y escalar durante holdSeconds
            float tElapsed = 0f;
            while (tElapsed < holdSeconds)
            {
                tElapsed += UnityEngine.Time.deltaTime;
                float k = -Mathf.Clamp01(tElapsed / holdSeconds);
                t.localScale = Vector3.LerpUnclamped(start, target, k); // o usa "f" si preferís la curva
                yield return null;
            }
            t.localScale = target;
        }
        else
        {
            // Si no hay Whole Doll, igual esperamos el tiempo para mantener el board visible
            yield return new UnityEngine.WaitForSeconds(holdSeconds);
        }

        // 4) Avisar que ya puede cerrarse (PuzzleManager destruye el board)
        onDone?.Invoke();
    }

}
