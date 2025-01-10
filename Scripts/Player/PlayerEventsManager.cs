using UnityEngine;

public class PlayerEventsManager : MonoBehaviour
{
    PlayerController playerController;
    bool isFallingRef, isGroundedRef;

    public delegate void HitGroundHandler();
    public event HitGroundHandler OnHitGround;

    public delegate void FallHandler();
    public event FallHandler OnFall;

    void Start()
    {
        playerController = GetComponent<PlayerController>();    
    }

    void Update()
    {
        Utils.OnVariableChange(playerController.isGrounded, ref isGroundedRef, (newValue) => 
        {
            if (newValue) OnHitGround?.Invoke(); 
        });


        Utils.OnVariableChange(playerController.isFalling, ref isFallingRef, (newValue) =>
        {
            if (newValue) OnFall?.Invoke();
        });
    }
}
