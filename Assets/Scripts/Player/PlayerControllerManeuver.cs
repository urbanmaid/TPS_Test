using System;
using System.Collections;
using UnityEngine;

public class PlayerControllerManeuver : MonoBehaviour
{
    private IPlayerController controller;
    private Rigidbody rb;

    [Header("General")]
    private float lerpDelayMin = 10f;
    [SerializeField] private float lerpDelay;
    private float lerpDelayMax = 56.25f;

    [Header("Move")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float moveSpeedWhenAim = 2.5f;
    [SerializeField] private float moveSpeedWhenCrouch = 3f;
    private Vector3 moveDirection;
    private Vector3 moveDirectionLerp;

    [Header("Look")]
    [SerializeField] private float lookSensitivity = 2f;
    private readonly double invRadian = Math.PI / 180.0;

    [Header("Jump")]
    [SerializeField] private int jumpAmount = 1;
    [SerializeField] private float jumpPower = 8f;
    private int jumpAmountCur;

    [Header("Slide")]
    [SerializeField] private float slidePower = 2.75f;
    private int slideAmountCur = 1;
    private readonly float slideDelay = 1.05f;

    public void InitializeMe(IPlayerController controller)
    {
        this.controller = controller;
        rb = GetComponent<Rigidbody>();

        lerpDelay = lerpDelayMin;
        jumpAmountCur = jumpAmount;
    }

    public void Move(Vector2 moveInput, bool isAiming)
    {
        if (controller.ManeuverStatus == 1)
        {
            moveDirection = new Vector3(
                moveInput.x * (float)Math.Cos(controller.LookDirectionX * invRadian) + moveInput.y * (float)Math.Sin(controller.LookDirectionX * invRadian),
                0f,
                moveInput.y * (float)Math.Cos(controller.LookDirectionX * invRadian) - moveInput.x * (float)Math.Sin(controller.LookDirectionX * invRadian)
            ).normalized * (isAiming ? moveSpeedWhenAim : (controller.IsCrouching ? moveSpeedWhenCrouch : moveSpeed));

            if (moveInput == Vector2.zero)
            {
                moveDirection = Vector3.zero;
            }
            moveDirectionLerp = Vector3.Lerp(moveDirectionLerp, moveDirection, Time.deltaTime * lerpDelayMin);

            if (controller.CanMove)
            {
                if (moveDirectionLerp.magnitude < 0.01f)
                {
                    rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
                }
                else
                {
                    rb.linearVelocity = new Vector3(moveDirectionLerp.x, rb.linearVelocity.y, moveDirectionLerp.z);
                }
                //transform.Translate(new Vector3(moveDirectionLerp.x, rb.linearVelocity.y, moveDirectionLerp.z) * Time.deltaTime);
            }
        }
    }

    public void SetDirection(Vector2 lookInput)
    {
        // Set X
        controller.LookDirectionX += lookInput.x * lookSensitivity * Time.deltaTime;
        if (controller.LookDirectionX > 180f) controller.LookDirectionX = -180f;
        if (controller.LookDirectionX < -180f) controller.LookDirectionX = 180f;

        // Set Y
        controller.LookDirectionY -= lookInput.y * lookSensitivity * Time.deltaTime;
        controller.LookDirectionY = Mathf.Clamp(controller.LookDirectionY, -80f, 80f);

        // Set Direction of Character X
        // This parameter is only changed when player move and on the ground
        if(controller.MoveInput.magnitude > 0.01f && controller.ManeuverStatus == 1)
        {
            /*
            controller.LookDirectionAvatarX = Quaternion.LookRotation(new Vector3(
                controller.MoveInput.x,
                0,
                controller.MoveInput.y
            )).eulerAngles.y + controller.LookDirectionX;
            */

            // If in aim state, player avatar follows the lookDirectionX Completely
            // If not, Set Direction Offset with Move Direction and add with current lookDirectionX
            controller.LookDirectionAvatarX = Mathf.LerpAngle(controller.LookDirectionAvatarX, 
            (!controller.IsAiming ? Quaternion.LookRotation(new Vector3(
                controller.MoveInput.x,
                0,
                controller.MoveInput.y
            )).eulerAngles.y : 0) + controller.LookDirectionX,
            lerpDelay * Time.deltaTime);
        }

        if(controller.IsAiming)
        {
            controller.LookDirectionAvatarX = Mathf.LerpAngle(controller.LookDirectionAvatarX, 
            controller.LookDirectionX,
            lerpDelay * Time.deltaTime);
        }

    }

    public void Jump()
    {
        if (jumpAmountCur > 0 && slideAmountCur > 0)
        {
            jumpAmountCur--;
            controller.ManeuverStatus = 3;

            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            rb.AddForce(Vector3.up * jumpPower + new Vector3(
                (float)Math.Sin(controller.LookDirectionAvatarX * invRadian), 
                0, 
                (float)Math.Cos(controller.LookDirectionAvatarX * invRadian)
            ) * slidePower, ForceMode.VelocityChange);
            controller.NotifyStateChange();
        }
    }

    public void Slide(bool isAiming)
    {
        if (!isAiming)
        {
            StartCoroutine(SlideCo());
        }
    }

    private IEnumerator SlideCo()
    {
        if (slideAmountCur > 0 && jumpAmountCur > 0 )
        {
            controller.ManeuverStatus = 2;
            slideAmountCur = 0;

            // Set Slide Direction which player avatar is heading
            rb.AddForce(1.25f * slidePower * new Vector3(
                (float)Math.Sin(controller.LookDirectionAvatarX * invRadian), 
                0, 
                (float)Math.Cos(controller.LookDirectionAvatarX * invRadian)
            ).normalized, ForceMode.VelocityChange);
            controller.NotifyStateChange();

            yield return new WaitForSeconds(slideDelay);
            controller.ManeuverStatus = 1;
            controller.NotifyStateChange();

            yield return new WaitForSeconds(slideDelay * 0.4f);
            slideAmountCur = 1;
        }
    }

    public void Crouch(bool value)
    {
        controller.IsCrouching = value;
    }

    public void SetAim(int maneuverStatus)
    {
        if (maneuverStatus == 1)
        {
            controller.IsAiming = true;
            controller.Camera?.SetAim();
            controller.NotifyStateChange();

            // Set Lerp Delay as Arm Mode
            StartCoroutine(SetLerpDelayAim(true));
        }
    }

    public void ResetAim()
    {
        controller.IsAiming = false;
        controller.Camera?.ResetAim();
        controller.NotifyStateChange();

        // Set Lerp Delay as Arm Mode
        StartCoroutine(SetLerpDelayAim(false));
    }

    // Changes Lerp Delay Continuously
    // True for Arm, False for Disarm
    private IEnumerator SetLerpDelayAim(bool value)
    {
        float valStart = value ? lerpDelayMin : lerpDelayMax;
        float valFinish = value ? lerpDelayMax : lerpDelayMin;
        float valReachingDuration = value ? 0.25f : 0.1f;
        float valCursor = 0;

        while(valCursor < 1)
        {
            valCursor += Time.deltaTime / valReachingDuration;
            lerpDelay = Mathf.Lerp(valStart, valFinish, valCursor);
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        ResetJump();
    }

    public void ResetJump()
    {
        controller.ManeuverStatus = 1;
        jumpAmountCur = jumpAmount;
        controller.NotifyStateChange();
    }
}