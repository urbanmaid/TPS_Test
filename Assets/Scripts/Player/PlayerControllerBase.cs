using System;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerControllerBase : MonoBehaviour, IPlayerController
{
    [SerializeField] protected PlayerControllerManeuver playerControllerManeuver;
    [SerializeField] protected PlayerControllerWeapon playerWeaponManager;
    protected PlayerControllerCamera playerControllerCamera;
    [SerializeField] protected PlayerControllerStatus playerControllerStatus;
    protected PlayerControllerAnimation playerControllerAnimation;
    protected InputSystem_Actions inputActions;
    protected Rigidbody rb;
    protected Vector3 initPos;

    [Header("Camera")]
    [SerializeField] protected GameObject playerCameraCoor;

    [Header("Maneuver")]
    public int ManeuverStatus { get; set; } = 1;
    public bool IsAiming { get; set; }
    public bool IsCrouching { get; set; }
    protected Vector2 moveInput;
    protected Vector2 lookInput;
    public float LookDirectionX { get; set; }
    public float LookDirectionAvatarX { get; set; }
    public float LookDirectionY { get; set; }
    public event Action<int, bool> OnManeuverStateChanged;

    public PlayerControllerCamera Camera => playerControllerCamera;
    public Vector2 MoveInput => moveInput;
    public Vector2 LookInput => lookInput;
    public abstract bool CanMove { get; }

    protected virtual void Awake()
    {
        SetInitialComponent();
        SetInitialInputAction();
    }

    protected void SetInitialComponent()
    {
        rb = GetComponent<Rigidbody>();
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

        LookDirectionAvatarX = transform.forward.x;
        initPos = transform.position;
        ManeuverStatus = 1;
    }

    protected void SetInitialInputAction()
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
        inputActions.Player.Crouch.performed += ctx => Crouch(true);
        inputActions.Player.Crouch.canceled += ctx => Crouch(false);

        inputActions.Player.Aim.performed += ctx => SetAim();
        inputActions.Player.Aim.canceled += ctx => ResetAim();

        inputActions.Player.Enable();
    }

    public void NotifyStateChange()
    {
        OnManeuverStateChanged?.Invoke(ManeuverStatus, IsAiming);
    }

    public abstract void Move();
    public abstract void SetDirection();
    public abstract void Jump();
    public abstract void Slide();
    public abstract void Crouch(bool value);
    public abstract void SetAim();
    public abstract void ResetAim();
    public abstract void UpdateMe();
}

public interface IPlayerController
{
    int ManeuverStatus { get; set; }
    bool IsAiming { get; set; }
    bool IsCrouching { get; set; }
    float LookDirectionX { get; set; }
    float LookDirectionAvatarX { get; set; }
    float LookDirectionY { get; set; }
    Vector2 MoveInput { get; }
    Vector2 LookInput { get; }
    PlayerControllerCamera Camera { get; }
    bool CanMove { get; } // 권한 체크용
    event Action<int, bool> OnManeuverStateChanged;
    void NotifyStateChange();
}