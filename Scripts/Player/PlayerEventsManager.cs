using UnityEngine;

public class PlayerEventsManager : MonoBehaviour
{
    PlayerController playerController;
    bool isFalling, isGrounded;

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
        Utils.OnVariableChange(playerController.isGrounded, ref isGrounded, () => 
        {
            if (isGrounded) OnHitGround?.Invoke(); 
        });


        Utils.OnVariableChange(playerController.isFalling, ref isFalling, () =>
        {
            if (isFalling) OnFall?.Invoke();
        });
    }
}
