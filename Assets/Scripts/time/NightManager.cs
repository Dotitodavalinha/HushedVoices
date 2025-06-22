using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightManager : MonoBehaviour
{
    public GameObject policias;

    public JailManager JailManager;
    public LightingManager DayManager;
    public bool IsNight;

    private void Start()
    {
        JailManager = GameObject.Find("JailManager")?.GetComponent<JailManager>();
    }
    private void Update()
    {
        if(DayManager.TimeOfDay > 20 || DayManager.TimeOfDay < 4)
        {
            IsNight = true;
        }
        else
        {
            IsNight = false;
        }
    }
    public void TalkingToPolice()
    {
        if (IsNight)
        {
            Debug.Log("hablaste con un policia de noche vas presitophite, CARCEEEL");
            ProgressManager.Instance.CambiarRootNPC("Police", "PoliceNight");
          //  JailManager.SetMaxValue();
        }
    }
}
