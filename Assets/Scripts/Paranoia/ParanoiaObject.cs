using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ParanoiaObject : MonoBehaviour
{
    [SerializeField] private Material _material;
    private static readonly int ParanoiaProperty = Shader.PropertyToID("_paranoia");

    void OnEnable()
    {
        // Se registra en el manager para ser actualizado
        ParanoiaManager.Instance?.RegisterParanoiaObject(this);
    }

    public void SetParanoia(float value)
    {
        _material.SetFloat(ParanoiaProperty, value);
    }
}