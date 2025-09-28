using System.Collections.Generic;
using UnityEngine;

public class UIInteractable : InteractableBase
{
    [Header("Prefabs de UI")]
    [SerializeField] private List<GameObject> prefabs = new List<GameObject>();


    private int currentIndex = 0;
    private GameObject currentInstance;

    protected override void OnActivate()
    {
        currentIndex = 0;
        Show(currentIndex);
    }

    protected override void OnActiveUpdate()
    {
        // Avanzar con E si hay varios prefabs
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (prefabs.Count > 1)
                Next();
            else
                Deactivate();
        }
    }

    protected override void OnDeactivate()
    {
        if (currentInstance != null)
            Destroy(currentInstance);
    }

    private void Show(int index)
    {
        if (index < 0 || index >= prefabs.Count) { Deactivate(); return; }

        if (currentInstance != null)
            Destroy(currentInstance);

        currentInstance = Instantiate(prefabs[index], canvasTransform);
        currentInstance.transform.SetAsLastSibling();
    }

    private void Next()
    {
        currentIndex++;
        if (currentIndex < prefabs.Count)
            Show(currentIndex);
        else
            Deactivate();
    }
}
