using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OsilacionCamera : MonoBehaviour
{
    public float wobbleIntensity = 0.1f;  // intensidad del movimiento
    public float wobbleSpeed = 1f;        // velocidad del movimiento

    Vector3 initialPosition;
    Vector3 initialRotation;

    void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localEulerAngles;
    }

    void Update()
    {
        float wobbleX = Mathf.PerlinNoise(Time.time * wobbleSpeed, 0f) - 0.5f;
        float wobbleY = Mathf.PerlinNoise(0f, Time.time * wobbleSpeed) - 0.5f;

        transform.localPosition = initialPosition + new Vector3(wobbleX, wobbleY, 0f) * wobbleIntensity;
    }
}
