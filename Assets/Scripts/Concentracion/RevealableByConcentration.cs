using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class RevealableByConcentration : MonoBehaviour
{
    [Header("Reveal settings")]
    public float revealRadius = 3f; // distancia al jugador para que se revele
    public bool requireLineOfSight = false;
    
    [Tooltip("Si es true, al revelarse se intenta agregar la pista automaticamente")]
    public bool AddClueOnReveal = true;

    // runtime
    private bool highlighted = false;
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        ConcentrationManager.Instance.OnConcentrationStarted += OnConcentrationStarted;
        ConcentrationManager.Instance.OnConcentrationEnded += OnConcentrationEnded;
    }

    private void OnDestroy()
    {
        if (ConcentrationManager.Instance != null)
        {
            ConcentrationManager.Instance.OnConcentrationStarted -= OnConcentrationStarted;
            ConcentrationManager.Instance.OnConcentrationEnded -= OnConcentrationEnded;
        }
    }

    private void OnConcentrationStarted()
    {
        if (player == null) return;
        float d = Vector3.Distance(player.position, transform.position);
        if (d <= revealRadius)
        {
            if (requireLineOfSight)
            {
                if (!HasLineOfSight()) return;
            }
            Highlight(true);
            if (AddClueOnReveal)
            {
                // registrar pista 
              
            }
        }
    }

    private void OnConcentrationEnded()
    {
        Highlight(false);
    }

    private void Highlight(bool on)
    {
        highlighted = on;
        // activa outline, particle o tweak visual del objeto en si

    }

    private bool HasLineOfSight()
    {
        Vector3 dir = (transform.position - player.position);
        RaycastHit hit;
        if (Physics.Raycast(player.position + Vector3.up * 1.6f, dir.normalized, out hit, revealRadius))
        {
            return hit.collider == GetComponent<Collider>();
        }
        return false;
    }
}
