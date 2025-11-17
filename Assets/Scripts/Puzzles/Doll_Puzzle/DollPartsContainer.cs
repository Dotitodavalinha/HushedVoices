using UnityEngine;

public class DollPartsContainer : MonoBehaviour
{
    public GameObject[] dollParts;

    bool activated = false;

    void Start()
    {
        foreach (var p in dollParts)
            if (p != null) p.SetActive(false);
    }

    void Update()
    {
        if (activated) return;

        if (PuzzleManager.Instance != null && PuzzleManager.Instance.dollQuestActive)
        {
            foreach (var p in dollParts)
                if (p != null) p.SetActive(true);

            activated = true;
        }
    }
}
