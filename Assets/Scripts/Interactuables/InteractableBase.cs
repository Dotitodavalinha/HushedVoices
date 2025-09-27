using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class InteractableBase : MonoBehaviour
{
    [Header("Opciones comunes")]
    [SerializeField] protected bool needInteract = true; // requiere apretar E
    [SerializeField] protected bool destroyAfterUse = false;
    [SerializeField] protected NOTEInteractionZone zonaInteraccion;
    [SerializeField] protected GameObject pressE_UI;

    protected bool inRange = false;
    protected bool active = false;
    protected Transform canvasTransform;
    private bool justStarted = false;

    protected virtual void Start()
    {
        AssignCanvas();
        if (pressE_UI != null)
            pressE_UI.SetActive(false);
    }

    protected virtual void Update()
    {
        if (zonaInteraccion != null && zonaInteraccion.jugadorDentro)
        {
            inRange = true;

            if (needInteract)
            {
                if (pressE_UI != null)
                    pressE_UI.SetActive(!active);

                if (!active && Input.GetKeyDown(KeyCode.E))
                {
                    if (TryActivate())
                        justStarted = true;
                }
            }
            else
            {
                if (!active)
                {
                    if (TryActivate())
                        justStarted = true;
                }
            }
        }
        else
        {
            inRange = false;
            if (pressE_UI != null)
                pressE_UI.SetActive(false);
        }

        // llamada de update propio de cada hijo
        if (active && !justStarted)
            OnActiveUpdate();

        justStarted = false;
    }

    protected bool TryActivate()
    {
        if (!GameManager.Instance.TryLockUI()) return false;

        active = true;
        OnActivate();
        return true;
    }

    protected void Deactivate()
    {
        active = false;
        GameManager.Instance.UnlockUI();
        OnDeactivate();

        if (pressE_UI != null)
            pressE_UI.SetActive(false);

        if (destroyAfterUse)
            Destroy(gameObject);
    }

    protected abstract void OnActivate();
    protected abstract void OnActiveUpdate();
    protected abstract void OnDeactivate();

    private void AssignCanvas()
    {
        var canvasObj = GameObject.Find("Canvas");
        if (canvasObj != null)
            canvasTransform = canvasObj.transform;
        else
            Debug.LogWarning("No se encontró un objeto llamado 'Canvas' en la escena.");
    }
}
