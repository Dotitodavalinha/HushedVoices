using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateLights : MonoBehaviour
{
    public LightingManager lightingManager;
    public GameObject dayLights;
    public GameObject nightLights;
    
    public GameObject activeLights;
    public GameObject pastLights;
    public float timer;
    public float minTime;
    public float maxTime;
    // Update is called once per frame
    void Start()
    {
        lightingManager = GameObject.Find("TimeManager")?.GetComponent<LightingManager>();
        timer = Random.Range(minTime, maxTime);
    }
    private void Update()
    {
        if(lightingManager.TimeOfDay > 20|| lightingManager.TimeOfDay < 5)
        {
            dayLights.SetActive(false);
            nightLights.SetActive(true);
            activateLights(nightLights);
        }
        else
        {
            dayLights.SetActive(true);
            nightLights.SetActive(false);
        }
    }
    void activateLights(GameObject lights)
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        if(timer <=0)
        {
            lights.SetActive(!lights.activeSelf);
            timer = Random.Range(minTime, maxTime);
        }
    }
}
