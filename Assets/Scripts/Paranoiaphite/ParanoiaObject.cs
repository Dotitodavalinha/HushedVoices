using UnityEngine;

public class ParanoiaObject : MonoBehaviour
{
    [SerializeField] private Material _material;
    private static readonly int ParanoiaProperty = Shader.PropertyToID("_paranoia");

    public void SetParanoia(float value)
    {
        _material.SetFloat(ParanoiaProperty, value);
    }
}
