using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightManager : MonoBehaviour
{
    public GameObject policias;
    public ParanoiaManager ParanoiaManager;
    public JailManager JailManager;
    public LightingManager DayManager;
    public bool IsNight;
    public GameObject ClockNit;
    public GameObject ClockDay;

    public GameObject IsNightAlert;
    [SerializeField] private Transform canvasTransform;
    private bool hasInstantiatedAlert = false;



    private void Start()
    {
        JailManager = GameObject.Find("JailManager")?.GetComponent<JailManager>();
        ParanoiaManager = GameObject.Find("ParanoiaManager")?.GetComponent<ParanoiaManager>();
        ClockDay = GameObject.Find("Dia");
        ClockNit = GameObject.Find("Noche");
        ClockNit.SetActive(false);
        
    }
    private void Update()
    {
        if (DayManager.TimeOfDay > 20 || DayManager.TimeOfDay < 5)
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
            JailManager.SetMaxValue();
        }
    }

    public void ClockDayTrue()
    {
        if (IsNight == true)
        {
            ParanoiaManager.SetParanoiaValue(-ParanoiaManager.lastParanoiaValue);
            IsNight = false;
        }
        ClockDay.SetActive(true);
        ClockNit.SetActive(false);
        hasInstantiatedAlert = false;
    }

    public void ClockNIghtTrue()
    {
        if (IsNight == false)
        {
            ParanoiaManager.lastParanoiaValue = 1f-ParanoiaManager.paranoiaLevel;
            ParanoiaManager.SetParanoiaValue(1);
            IsNight = true;
        }
        if (!hasInstantiatedAlert)
        {
            Instantiate(IsNightAlert, canvasTransform);
            hasInstantiatedAlert = true;
        }
        ClockDay.SetActive(false);
        ClockNit.SetActive(true);
        
        

    }

}
