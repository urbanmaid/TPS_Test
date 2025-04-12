using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerControllerCamera : MonoBehaviour
{
    [SerializeField] GameObject[] cameraPositionCriterion;
    [SerializeField] GameObject[] cameraPositionCriterionWhenAiming;
    private PlayerControllerManeuver maneuver;
    private GameObject deathCamCinemachine;

    [SerializeField] int cameraPosIndex = 0;
    int cameraPosIndexMax;
    float shakeDuration = 0.15f;
    float shakeMagnitude = 0.1f;

    // Initialization of camera that defines integrity
    internal void InitializeMe(PlayerControllerManeuver playerControllerManeuver)
    {
        maneuver = playerControllerManeuver;
        cameraPosIndexMax = cameraPositionCriterion.Length - 1;
        if(cameraPosIndexMax == -1)
        {
            Debug.LogError("Shibal Assign at least one camera on coordinate");
        }

        SetFirstCamera();
    }

    void SetFirstCamera()
    {
        cameraPositionCriterion[0].GetComponent<CinemachineCamera>().Priority = 10;
    }

    // Swap camera via tuning up property
    internal void SwapCameraPos()
    {
        cameraPositionCriterion[cameraPosIndex].GetComponent<CinemachineCamera>().Priority = 1;
        if(cameraPosIndex == cameraPosIndexMax) // If reached to max of list resets to 0
        {
            cameraPosIndex = 0;
        }
        else
        {
            cameraPosIndex++;
        }
        cameraPositionCriterion[cameraPosIndex].GetComponent<CinemachineCamera>().Priority = 10;
        
        //Debug.Log("Changed camera pos as: " + cameraPositionCriterion[cameraPosIndex]);
    }
    
    internal void SetAim()
    {
        cameraPositionCriterionWhenAiming[cameraPosIndex].GetComponent<CinemachineCamera>().Priority = 10;
        cameraPositionCriterion[cameraPosIndex].GetComponent<CinemachineCamera>().Priority = 1;
        Debug.Log("Aiming");
    }

    internal void ResetAim()
    {
        cameraPositionCriterion[cameraPosIndex].GetComponent<CinemachineCamera>().Priority = 10;
        cameraPositionCriterionWhenAiming[cameraPosIndex].GetComponent<CinemachineCamera>().Priority = 1;
    }

    // Show damage with cam
    public IEnumerator ShakeCamera(ICinemachineCamera targetCamInterface)
    {
        // Shakes camera when damaged by modifying following offset as randomized Vector3
        // target is GameManager.instance.mainCamera.ActiveVirtualCamera(ICinemachineCamera) but types of input will be CinemachineCamera mostly
        // and resets offset as Vector3.zero
        // As using IEnumerator

        if (targetCamInterface == null) yield break;

        CinemachineCamera targetCam = targetCamInterface as CinemachineCamera; // Get real camera from interface
        Transform targetCamTransform = targetCam.gameObject.transform; // Get camera transform
        Vector3 initPosOfCam = targetCamTransform.localPosition; // Set camera initial position

        float shakeTimeElapsed = 0;

        Debug.Log(targetCamInterface + " is using when damaged, Initial pos: " + initPosOfCam);

        while (shakeTimeElapsed < shakeDuration)
        {
            shakeTimeElapsed += Time.deltaTime;
            targetCamTransform.localPosition = initPosOfCam + RandomShake();
            yield return null;
        }

        targetCamTransform.localPosition = initPosOfCam;
    }

    Vector3 RandomShake()
    {
        return new Vector3
        (
            Random.Range(-shakeMagnitude, shakeMagnitude),
            Random.Range(-shakeMagnitude, shakeMagnitude),
            Random.Range(-shakeMagnitude, shakeMagnitude)
        );
    }

    // Show death cam via adding prefab of cinemachine Camera
    internal void SetDeathCam(float camDist)
    {
        deathCamCinemachine = GameManager.instance.InstanciateDeathCam(transform.position, camDist);
        deathCamCinemachine.GetComponent<CinemachineCamera>().LookAt = gameObject.transform;
        Debug.Log(deathCamCinemachine + " has been spawned");
    }

    internal void ResetDeathCam()
    {
        Destroy(deathCamCinemachine);
    }
}
