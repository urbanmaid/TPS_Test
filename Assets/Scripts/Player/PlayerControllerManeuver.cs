using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerManeuver : MonoBehaviour
{
    #region Variables

    [SerializeField] PlayerControllerWeapon playerWeaponManager;
    [SerializeField] GameObject playerCameraCoor;
    internal PlayerControllerCamera playerControllerCamera;
    [SerializeField] PlayerControllerStatus playerControllerStatus;
    InputSystem_Actions inputActions;
    Rigidbody rb;
    Vector3 initPos;

    [Header("Maneuver")]
    [Tooltip("0 for death, 1 for alive, 2 for immovilized")]
    public int maneuverStatus = 1;
    [Space]
    [SerializeField] float lerpDelay = 8f;
    [SerializeField] float moveSpeed = 8f;
    private Vector2 moveInput;
    private Vector3 moveDirection;
    private Vector3 moveDirectionLerp;

    [Space]
    [Tooltip("lookInput defines Input of camera direction, X for Horizontal, Y for vertical.")]
    [SerializeField] Vector2 lookInput; //Input of camera direction, X for input
    [SerializeField] float lookSensitivity = 2f;
    private float lookDirection;

    [Space]
    [SerializeField] int jumpAmount = 1;
    private int jumpAmountCur;
    [SerializeField] float jumpPower = 8f;

    private readonly double invRadian = (Math.PI / 180.0);

    #endregion
    #region Initialization

    void Start()
    {
        SetInitialComponenet();
        SetInitialInputAction();
    }

    void SetInitialComponenet()
    {
        // Components
        rb = GetComponent<Rigidbody>();
        if(rb == null)
        {
            Debug.LogError("There are no Rigidbodies on player");
        }

        // Get other non-rb components
        playerWeaponManager = GetComponent<PlayerControllerWeapon>();
        playerControllerCamera = GetComponent<PlayerControllerCamera>();
        playerControllerCamera.InitializeMe(this);
        playerControllerStatus = GetComponent<PlayerControllerStatus>();
        playerControllerStatus.InitializeMe(this);

        // Variables
        jumpAmountCur = jumpAmount;
        initPos = transform.position;
    }

    void SetInitialInputAction()
    {
        inputActions = new InputSystem_Actions();
        if(inputActions != null)
        {
            Debug.Log("inputActions has been loaded");
        }

        // Move
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        // Look
        inputActions.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();

        // Jump
        inputActions.Player.Jump.performed += ctx => Jump();

        // Swap cam
        inputActions.Player.CameraSwap.performed += ctx => playerControllerCamera.SwapCameraPos();

        inputActions.Player.Enable();
    }

    internal void Respawn()
    {
        transform.position = initPos;
        maneuverStatus = 1;

        Debug.Log("Respawned");
    }

    public void UpdateMe()
    {
        Move();
        SetDirection();
    }

    #endregion
    #region Maneuver
    
    void Move()
    {
        if(maneuverStatus != 0)
        {
            /*
            X - moveInput.x * sin(lookDirection) + ??
            Y - 0
            Z - moveInput.y * cos(lookDirection) + ??
            whole math function is operated by radian so converted as invRadian
            */
            moveDirection = new Vector3(
                moveInput.x * (float)Math.Cos(lookDirection * invRadian) + moveInput.y * (float)Math.Sin(lookDirection * invRadian), 
                0f, 
                moveInput.y * (float)Math.Cos(lookDirection * invRadian) - moveInput.x * (float)Math.Sin(lookDirection * invRadian)
                ).normalized * moveSpeed;
            moveDirectionLerp = Vector3.Lerp(moveDirectionLerp, moveDirection, Time.deltaTime * lerpDelay);

            //rotate the coordinate of movDirection as new Vector3(0f, lookDirection, 0f)
            rb.linearVelocity = new Vector3(moveDirectionLerp.x, rb.linearVelocity.y, moveDirectionLerp.z);
        }
    }

    void SetDirection()
    {
        lookDirection = lookInput.x * lookSensitivity * 0.1f;
        
        // Set Camera Direction
        playerCameraCoor.transform.rotation = Quaternion.Euler(
            -Mathf.Clamp(lookInput.y * lookSensitivity * 0.1f, -80f, 80f), 
            lookDirection, 0f
        );
    }

    private void Jump()
    {
        if(jumpAmountCur > 0)
        {
            jumpAmountCur--;
            rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
            Debug.Log("Jumping Now");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        jumpAmountCur = jumpAmount;
        Debug.Log("Landed");
    }
    #endregion
}
