using Unity.Cinemachine;
using UnityEngine;

public class PlayerControllerCamera : MonoBehaviour
{
    [SerializeField] GameObject[] cameraPositionCriterion;
    //[SerializeField] Animator cameraSwapAnimator;
    [SerializeField] CinemachineCamera cineCam;
    int cameraPosIndex = 0;
    int cameraPosIndexMax;

    void Start()
    {
        cameraPosIndexMax = cameraPositionCriterion.Length - 1;
        if(cameraPosIndexMax == -1)
        {
            Debug.LogError("Shibal Assign at least one camera on coordinate");
        }
    }

    // Update is called once per frame
    internal void SwapCameraPos()
    {
        if(cameraPosIndex == cameraPosIndexMax)
        {
            cameraPosIndex = 0;
        }
        else
        {
            cameraPosIndex++;
        }
        Debug.Log("Changed camera pos as: " + cameraPositionCriterion[cameraPosIndex]);
        cineCam.Follow = cameraPositionCriterion[cameraPosIndex].transform;
        /*
        if(cameraPosIndex == 1)
        {
            cameraSwapAnimator.SetInteger("CameraState", 3);
            cameraPosIndex = 3;
        }
        else
        {
            cameraSwapAnimator.SetInteger("CameraState", 1);
            cameraPosIndex = 1;
        }
        */
    }
}
