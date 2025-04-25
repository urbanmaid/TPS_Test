using System;
using Fusion;
using UnityEngine;

public class PlayerControllerBaseNetwork : NetworkBehaviour, IPlayerController
{
    [Networked] public int ManeuverStatus { get; set; }
    [Networked] public NetworkBool IsAiming { get; set; }
    [Networked] public NetworkBool IsCrouching { get; set; }
    [Networked] public float LookDirectionX { get; set; }
    [Networked] public float LookDirectionAvatarX { get; set; }
    [Networked] public float LookDirectionY { get; set; }
    [Networked] private Vector3 InitPos { get; set; }

    [SerializeField] protected PlayerControllerManeuver playerControllerManeuver;
    [SerializeField] private PlayerControllerWeapon playerWeaponManager;
    private PlayerControllerCamera playerControllerCamera;
    [SerializeField] private PlayerControllerStatus playerControllerStatus;
    private PlayerControllerAnimation playerControllerAnimation;
    private InputSystem_Actions inputActions;
    private Vector2 moveInput;
    private Vector2 lookInput;
    [SerializeField] private GameObject playerCameraCoor;

    public event Action<int, bool> OnManeuverStateChanged;
    public bool CanMove => Object.HasStateAuthority;
    public PlayerControllerCamera Camera => playerControllerCamera;
    public Vector2 MoveInput => moveInput;
    public Vector2 LookInput => lookInput;

    public int maneuverStatus { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    bool IPlayerController.IsAiming { get => IsAiming; set => IsAiming = value; }
    bool IPlayerController.IsCrouching { get => IsCrouching; set => IsCrouching = value; }

    private PlayerControllerManeuver maneuver;

    public override void Spawned()
    {
        SetInitialComponent();
        maneuver = gameObject.AddComponent<PlayerControllerManeuver>();
        maneuver.InitializeMe(this);

        if (Object.HasInputAuthority)
        {
            SetInitialInputAction();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            if (playerCameraCoor != null)
            {
                playerCameraCoor.SetActive(false);
            }
            Debug.Log($"다른 플레이어 오브젝트: 카메라 비활성화, PlayerRef={Object.InputAuthority}");
        }

        InitPos = transform.position;
        ManeuverStatus = 1;
    }

    private void SetInitialComponent()
    {
        var rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("No Rigidbody on player");
        }
        playerControllerManeuver = GetComponent<PlayerControllerManeuver>();
        playerControllerManeuver?.InitializeMe(this);
        playerWeaponManager = GetComponent<PlayerControllerWeapon>();

        playerControllerCamera = GetComponent<PlayerControllerCamera>();
        playerControllerCamera?.InitializeMe(this);
        playerControllerStatus = GetComponent<PlayerControllerStatus>();
        playerControllerStatus?.InitializeMe(this);
        playerControllerAnimation = GetComponent<PlayerControllerAnimation>();
        playerControllerAnimation?.InitializeMe(this);
    }

    private void SetInitialInputAction()
    {
        inputActions = new InputSystem_Actions();
        if (inputActions != null)
        {
            Debug.Log("InputActions loaded");
        }

        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputActions.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        inputActions.Player.Jump.performed += ctx => Jump();
        inputActions.Player.Slide.performed += ctx => Slide();
        inputActions.Player.Aim.performed += ctx => SetAim();
        inputActions.Player.Aim.canceled += ctx => ResetAim();

        inputActions.Player.Enable();
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasInputAuthority)
        {
            Move();
            SetDirection();
        }
        UpdateMe();
    }

    public void UpdateMe()
    {
        playerControllerAnimation?.SetDirection();

        if (Object.HasInputAuthority && playerCameraCoor != null)
        {
            playerCameraCoor.transform.rotation = Quaternion.Euler(LookDirectionY, LookDirectionX, 0f);
        }
    }

    public void Move()
    {
        maneuver.Move(MoveInput, IsAiming);
    }

    public void SetDirection()
    {
        maneuver.SetDirection(LookInput);
    }

    public void Jump()
    {
        if (Object.HasStateAuthority)
        {
            maneuver.Jump();
            RPC_NotifyStateChange(ManeuverStatus, IsAiming);
        }
    }

    public void Slide()
    {
        if (Object.HasStateAuthority)
        {
            maneuver.Slide(IsAiming);
            RPC_NotifyStateChange(ManeuverStatus, IsAiming);
        }
    }

    public void Crouch(bool value)
    {
        if (Object.HasStateAuthority)
        {
            maneuver.Crouch(value);
        }
    }

    public void SetAim()
    {
        if (Object.HasStateAuthority)
        {
            maneuver.SetAim(ManeuverStatus);
            RPC_NotifyStateChange(ManeuverStatus, IsAiming);
        }
    }

    public void ResetAim()
    {
        if (Object.HasStateAuthority)
        {
            maneuver.ResetAim();
            RPC_NotifyStateChange(ManeuverStatus, IsAiming);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_NotifyStateChange(int maneuverStatus, NetworkBool isAiming)
    {
        ManeuverStatus = maneuverStatus;
        IsAiming = isAiming;
        NotifyStateChange();
        Debug.Log($"상태 동기화: ManeuverStatus={maneuverStatus}, IsAiming={isAiming}");
    }

    public void NotifyStateChange()
    {
        OnManeuverStateChanged?.Invoke(ManeuverStatus, IsAiming);
    }
}