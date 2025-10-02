using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    Vector3 startRunPos;

    public void PlayRunAnimation()
    {
        startRunPos = transform.position;
    }

    public void EndRunAnimation()
    {
        transform.position = startRunPos;
    }
}
