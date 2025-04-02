using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerManeuver : MonoBehaviour
{
    [SerializeField] PlayerControllerWeapon playerWeaponManager;
    [SerializeField] GameObject playerCameraCoor;
    private PlayerControllerCamera playerControllerCamera;
    InputSystem_Actions inputActions;
    Rigidbody rb;

    [Header("Maneuver")]
    [SerializeField] float lerpDelay = 8f;
    [SerializeField] float moveSpeed = 8f;
    private Vector2 moveInput;
    private Vector3 moveDirection;
    private Vector3 moveDirectionLerp;

    [Tooltip("lookInput defines Input of camera direction, X for Horizontal, Y for vertical.")]
    private Vector2 lookInput; //Input of camera direction, X for input
    [SerializeField] float lookSensitivity = 2f;
    private float lookDirection;

    [Space]
    [SerializeField] int jumpAmount = 1;
    [SerializeField] float jumpPower = 8f;
    private int jumpAmountCur;
    private readonly double invRadian = (Math.PI / 180.0);

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

        playerWeaponManager = GetComponent<PlayerControllerWeapon>();
        playerControllerCamera = GetComponent<PlayerControllerCamera>();

        // Variables
        jumpAmountCur = jumpAmount;
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

    void Update()
    {
        Move();
        SetDirection();
    }

    #region Maneuver
    void Move()
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
