using UnityEngine;

public class DraggablePart : MonoBehaviour
{
    public string targetId;
    public float snapDistance = 0.25f;

    Vector3 startPos;
    bool placed;
    Camera cam;
    SocketZone hovered;

    void Awake() { cam = Camera.main; startPos = transform.position; }

    void OnMouseDrag()
    {
        if (placed) return;
        Vector3 m = cam.ScreenToWorldPoint(Input.mousePosition);
        m.z = 0f;
        transform.position = m;
    }

    void OnMouseUp()
    {
        if (placed) return;
        if (hovered != null && hovered.id == targetId &&
            Vector2.Distance(transform.position, hovered.transform.position) <= snapDistance)
        {
            transform.position = hovered.transform.position;
            placed = true;
            GetComponent<Collider2D>().enabled = false;
            var rb = GetComponent<Rigidbody2D>(); if (rb) rb.simulated = false;
            PuzzleManager.Instance.NotifyPlaced();
        }
        else
        {
            transform.position = startPos;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out SocketZone socket)) hovered = socket;
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out SocketZone socket) && hovered == socket) hovered = null;
    }
}
