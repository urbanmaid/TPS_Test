using UnityEngine;

public class PlayerControllerBaseLocal : PlayerControllerBase
{
    protected override void Awake()
    {
        base.Awake();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Move();
        SetDirection();
        UpdateMe();
    }

    public override bool CanMove => true; // 로컬 모드는 항상 이동 가능

    public override void UpdateMe()
    {
        playerControllerAnimation?.SetDirection();
        
        if (playerCameraCoor != null)
        {
            playerCameraCoor.transform.rotation = Quaternion.Euler(LookDirectionY, LookDirectionX, 0f);
        }
    }

    public override void Move()
    {
        playerControllerManeuver.Move(MoveInput, IsAiming);
    }

    public override void SetDirection()
    {
        playerControllerManeuver.SetDirection(LookInput);
    }

    public override void Jump()
    {
        playerControllerManeuver.Jump();
    }

    public override void Slide()
    {
        playerControllerManeuver.Slide(IsAiming);
    }

    public override void Crouch(bool value)
    {
        playerControllerManeuver.Crouch(value);
    }

    public override void SetAim()
    {
        playerControllerManeuver.SetAim(ManeuverStatus);
    }

    public override void ResetAim()
    {
        playerControllerManeuver.ResetAim();
    }
}