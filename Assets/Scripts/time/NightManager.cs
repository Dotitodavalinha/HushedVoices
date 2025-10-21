using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.BoolParameter;

public class NightManager : MonoBehaviour
{
    public GameObject policias; 
    public ParanoiaManager ParanoiaManager;
    public JailManager JailManager;
    public LightingManager DayManager;
    public bool IsNight;

    public GameObject IsNightAlert;
    public GameObject IsPreNightAlert;

    [SerializeField] private Transform canvasTransform;
    private bool hasInstantiatedAlert = false;
    [SerializeField] public bool InstantiatedPreNightAlert = false;

    private void Start()
    {
        JailManager = GameObject.Find("JailManager")?.GetComponent<JailManager>();
        ParanoiaManager = GameObject.Find("ParanoiaManager")?.GetComponent<ParanoiaManager>();

        SoundManager.instance.PlayMusic(MusicID.StaticSound, true);
        SoundManager.instance.ChangeVolumeOneMusic(MusicID.StaticSound, -1f); //arranca en 0
    }

    private void Update()
    {
        if (DayManager.TimeOfDay > 20 || DayManager.TimeOfDay < 5)
        {
            ClockNIghtTrue();
            SoundManager.instance.ChangeMusic(MusicID.NightSound);
        }
        else
        {
            SoundManager.instance.ChangeMusic(MusicID.DaySound);
            ClockDayTrue();
        }
        if (DayManager.TimeOfDay >= 19 && DayManager.TimeOfDay <= 19.1)
        {
            AlmostNight();
        }
        else
        {
            InstantiatedPreNightAlert = false;
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
            // Usa el nuevo método directo para resetear a 0
            ParanoiaManager.SetParanoiaValueDirect(0f);
            IsNight = false;
        }
        hasInstantiatedAlert = false;
    }

    public void ClockNIghtTrue()
    {
        if (IsNight == false)
        {
            SoundManager.instance.ChangeVolumeOneMusic(MusicID.StaticSound, 0.2f);
            // Usa el nuevo método directo para establecer a 1
            ParanoiaUp();
            //ParanoiaManager.SetParanoiaValueDirect(1f);
            IsNight = true;
        }
        if (!hasInstantiatedAlert)
        {
            SoundManager.instance.PlaySound(SoundID.alarm, false, 0.15f);
            Instantiate(IsNightAlert, canvasTransform);
            hasInstantiatedAlert = true;
        }

    }
    void ParanoiaUp ()
    {
        //save me
        for(float i=ParanoiaManager.paranoiaLevel; i <1f; i++)
        {
            ParanoiaManager.SetParanoiaValueDirect(Mathf.Lerp(i, 1f, i));
        }
    }

    private void AlmostNight()
    {
        if (!InstantiatedPreNightAlert)
        {
            Instantiate(IsPreNightAlert, canvasTransform);
            InstantiatedPreNightAlert = true;
        }
    }

    public void ResetManager()
    {
        IsNight = false;
        hasInstantiatedAlert = false;
        InstantiatedPreNightAlert = false;
    
    }



}