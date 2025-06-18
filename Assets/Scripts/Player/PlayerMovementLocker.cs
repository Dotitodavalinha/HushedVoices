using UnityEngine;

public class PlayerMovementLocker : MonoBehaviour
{

    [SerializeField] private Player_Movement player;

    
    public void LockMovement()
    {
        
            player.enabled = false;

        
    }

    public void UnlockMovement()
    {

        player.enabled = true;

    }
}
