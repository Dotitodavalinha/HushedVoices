using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightManager : MonoBehaviour
{
    public GameObject policias;

    public JailManager JailManager;
    public LightingManager DayManager;
    public bool IsNight;
    public GameObject ClockNit;
    public GameObject ClockDay;

    public GameObject IsNightAlert;


    private void Start()
    {
        JailManager = GameObject.Find("JailManager")?.GetComponent<JailManager>();
        ClockDay = GameObject.Find("Dia");
        ClockNit = GameObject.Find("Noche");
        ClockNit.SetActive(false);
    }
    private void Update()
    {
        if (DayManager.TimeOfDay > 20 || DayManager.TimeOfDay < 6)
        {
            
            ClockNIghtTrue();

        }
        else
        {
            
            ClockDayTrue();
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

    public void ClockDayTrue()
    {
        IsNight = false;
        ClockDay.SetActive(true);
        ClockNit.SetActive(false);
    }

    public void ClockNIghtTrue()
    {
       
        Instantiate(IsNightAlert); //hacer q no se llame a clocknight en el update y q esto se instancie done el canvas, igual q en ImportantClue

        IsNight = true;
        ClockDay.SetActive(false);
        ClockNit.SetActive(true);
    }

}
