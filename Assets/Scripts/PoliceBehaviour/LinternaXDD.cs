using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinternaXDD : MonoBehaviour
{
    [SerializeField] private NightManager nightManager;
    [SerializeField] public GameObject Linterna;

    [SerializeField] private Animator animator;

    [Header("Controllers")]
    [SerializeField] private RuntimeAnimatorController dayController;
    [SerializeField] private RuntimeAnimatorController nightController;

    void Start()
    {
        nightManager = FindObjectOfType<NightManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Linterna.SetActive(nightManager.IsNight);

        if (nightManager.IsNight)
        {
            if (animator.runtimeAnimatorController != nightController)
                animator.runtimeAnimatorController = nightController;
        }
        else
        {
            if (animator.runtimeAnimatorController != dayController)
                animator.runtimeAnimatorController = dayController;
        }
    }
}
