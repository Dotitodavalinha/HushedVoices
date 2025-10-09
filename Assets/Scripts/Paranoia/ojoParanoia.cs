using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ojoParanoia : MonoBehaviour
{
    public GameObject closed;
    public GameObject aClosed;
    public GameObject aOpen;
    public GameObject open;

    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void setSprite (float value)
    {

        if (value <= 0)
        {
            closed.SetActive(true);
            aClosed.SetActive(false);
            aOpen.SetActive(false);
            open.SetActive(false);
        }
        else if (0.25 >= value && value > 0)
        {
            closed.SetActive(false);
            aClosed.SetActive(true);
            aOpen.SetActive(false);
            open.SetActive(false);
        }
        else if (0.5 >= value && value > 0.25)
        {
            closed.SetActive(false);
            aClosed.SetActive(false);
            aOpen.SetActive(true);
            open.SetActive(false);
        }
        else if (1 >= value && value > 0.5)
        {
            closed.SetActive(false);
            aClosed.SetActive(false);
            aOpen.SetActive(false);
            open.SetActive(true);
        }
    }
}
