using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimLuzTele : MonoBehaviour
{
    public Light myLight;

    public void masIntensidad()
    {
        myLight.intensity += 0.15f;
    }

    public void menosIntensidad()
    {
        myLight.intensity -= 0.15f;
    }
}
