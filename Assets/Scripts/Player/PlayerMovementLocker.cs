using UnityEngine;

public class PlayerMovementLocker : MonoBehaviour
{
  
    [SerializeField] private Player_Movement player;

    //[SerializeField] private bool isLocked = false;
   

    public void LockMovement()
    {       
            player.enabled = false;       
    }

    public void UnlockMovement()
    {
                
            player.enabled = true;
        
    }
}
