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
        float yOffset = Mathf.Sin(Time.time * wobbleSpeed) * wobbleIntensity;
        transform.localPosition = initialPosition + new Vector3(0f, yOffset, 0f);
    }


}
