using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerManeuver : MonoBehaviour
{
    #region Variables

    [SerializeField] PlayerControllerWeapon playerWeaponManager;
    internal PlayerControllerCamera playerControllerCamera;
    [SerializeField] PlayerControllerStatus playerControllerStatus;
    PlayerControllerAnimation playerControllerAnimation;
    InputSystem_Actions inputActions;
    Rigidbody rb;
    Vector3 initPos;

    [Header("Camera")]
    [SerializeField] GameObject playerCameraCoor;

    [Header("Maneuver")]
    [Tooltip("0 for death, 1 for alive, 2 for immovilized. 3 for jumping")]
    public int maneuverStatus = 1; // Sets player movement
    public bool isAiming = false; // Sets player aim pose
    [Space]
    [SerializeField] float lerpDelay = 8f;
    [SerializeField] float moveSpeed = 8f;
    [SerializeField] float moveSpeedWhenAim = 2.25f;
    private Vector2 moveInput;
    private Vector3 moveDirection;
    private Vector3 moveDirectionLerp;

    [Space]
    [Tooltip("lookInput defines Input of camera direction, X for Horizontal, Y for vertical.")]
    private Vector2 lookInput; //Input of camera direction, X for input
    [SerializeField] float lookSensitivity = 2f;
    public float lookDirectionX;
    [SerializeField] float lookDirectionY;

    [Space]
    [SerializeField] int jumpAmount = 1;
    private int jumpAmountCur;
    [SerializeField] float jumpPower = 8f;
    [SerializeField] float slidePower = 2.75f;
    private int slideAmountCur = 1;
    private float slideDelay = 1.05f;
    private readonly double invRadian = (Math.PI / 180.0);

    //[Header("Animation Connection")]
    public event Action<int, bool> OnManeuverStateChanged;

    #endregion
    #region Initialization

    void Start()
    {
        SetInitialComponenet();
        SetInitialInputAction();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
        playerControllerAnimation = GetComponent<PlayerControllerAnimation>();
        playerControllerAnimation.InitializeMe(this);

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
        inputActions.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        // Jump
        inputActions.Player.Jump.performed += ctx => Jump();
        inputActions.Player.Slide.performed += ctx => Slide();

        // Swap cam
        inputActions.Player.Aim.performed += ctx => SetAim();
        inputActions.Player.Aim.canceled += ctx => ResetAim();

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

        // External Action
        playerControllerAnimation.SetDirection(lookDirectionX);
    }

    #endregion
    #region Maneuver
    
    void Move()
    {
        if(maneuverStatus == 1)
        {
            /*
            X - moveInput.x * sin(lookDirection) + ??
            Y - 0
            Z - moveInput.y * cos(lookDirection) + ??
            whole math function is operated by radian so converted as invRadian
            */
            moveDirection = new Vector3(
                moveInput.x * (float)Math.Cos(lookDirectionX * invRadian) + moveInput.y * (float)Math.Sin(lookDirectionX * invRadian), 
                0f, 
                moveInput.y * (float)Math.Cos(lookDirectionX * invRadian) - moveInput.x * (float)Math.Sin(lookDirectionX * invRadian)
                ).normalized
                 * (isAiming? moveSpeedWhenAim: moveSpeed);
            moveDirectionLerp = Vector3.Lerp(moveDirectionLerp, moveDirection, Time.deltaTime * lerpDelay);

            //rotate the coordinate of movDirection as new Vector3(0f, lookDirection, 0f)
            rb.linearVelocity = new Vector3(moveDirectionLerp.x, rb.linearVelocity.y, moveDirectionLerp.z);
        }
    }

    void SetDirection()
    {
        // Set value of Rotate Direction X
        lookDirectionX += lookInput.x * lookSensitivity * Time.deltaTime;
        if (lookDirectionX > 180f) lookDirectionX = -180f;
        if (lookDirectionX < -180f) lookDirectionX = 180f;

        // Set value of Rotate Direction Y
        lookDirectionY -= lookInput.y * lookSensitivity * Time.deltaTime;
        lookDirectionY = Mathf.Clamp(lookDirectionY, -80f, 80f);
        
        // Set Camera Direction
        playerCameraCoor.transform.rotation = Quaternion.Euler(
            lookDirectionY, 
            lookDirectionX, 0f
        );
    }

    #endregion
    #region Additional Action
    private void Jump()
    {
        if(jumpAmountCur > 0)
        {
            jumpAmountCur--;
            maneuverStatus = 3;
            rb.AddForce(Vector3.up * jumpPower + new Vector3(moveDirection.x, 0, moveDirection.z).normalized * (jumpPower / 2), ForceMode.VelocityChange);
            //Debug.Log("Jumping Now");
        }
    }

    private void Slide()
    {
        if(!isAiming)
        {
            StartCoroutine(SlideCo());
        }
    }

    // Coroutine of Slide
    IEnumerator SlideCo()
    {
        if(slideAmountCur > 0)
        {
            maneuverStatus = 2;
            slideAmountCur = 0;
            rb.AddForce(new Vector3(moveDirection.x, 0, moveDirection.z).normalized * slidePower, ForceMode.VelocityChange);
            
            yield return new WaitForSeconds(slideDelay);
            maneuverStatus = 1;

            yield return new WaitForSeconds(slideDelay * 0.4f);
            slideAmountCur = 1;
        }
    }

    void SetAim()
    {
        if(maneuverStatus == 1)
        {
            isAiming = true;
            if(playerControllerCamera) playerControllerCamera.SetAim();
        }
    }

    void ResetAim()
    {
        isAiming = false;
        if(playerControllerCamera) playerControllerCamera.ResetAim();
    }

    private void NotifyStateChange()
    {
        OnManeuverStateChanged?.Invoke(maneuverStatus, isAiming);
    }

    void OnCollisionEnter(Collision collision)
    {
        maneuverStatus = 1;
        jumpAmountCur = jumpAmount;
        //Debug.Log("Landed");
    }
    #endregion
}
