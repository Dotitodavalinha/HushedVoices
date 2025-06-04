using UnityEngine;
using System;
using System.Collections.Generic;

public class ProgressManager : MonoBehaviour
{

    public static ProgressManager Instance;

    public bool BensNoteUnlocked;
    public bool CoffeeShopUnlocked;

    public event Action OnBensNoteUnlocked;
    public event Action OnCoffeeShopUnlocked;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UnlockBensNote()
    {
        BensNoteUnlocked = true;
        OnBensNoteUnlocked?.Invoke();
    }

    public void UnlockCoffeeShop()
    {
        CoffeeShopUnlocked = true;
        OnCoffeeShopUnlocked?.Invoke();
    }
}
