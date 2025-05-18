using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ProgressManager : MonoBehaviour
{
    
    public static ProgressManager Instance;

    public bool BensNoteUnlocked;
    public bool CoffeeShopUnlocked;
    // mas bools según los avances

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

}
