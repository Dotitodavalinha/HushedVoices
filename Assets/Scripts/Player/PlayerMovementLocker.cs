using UnityEngine;

public class PlayerMovementLocker : MonoBehaviour
{

    [SerializeField] private Player_Movement player;

    
    public void LockMovement()
    {
        player.anim.SetFloat("Speed", 0);

        player.enabled = false;

        
    }

    public void UnlockMovement()
    {

        player.enabled = true;

    }
}
