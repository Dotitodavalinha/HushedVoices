using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinternaXDD : MonoBehaviour
{
    [SerializeField] private NightManager nightManager;
    [SerializeField] public GameObject Linterna;

    void Start()
    {
        nightManager = FindObjectOfType<NightManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Linterna.SetActive(nightManager.IsNight);
    }
}
