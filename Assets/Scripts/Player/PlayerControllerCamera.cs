using Unity.Cinemachine;
using UnityEngine;

public class PlayerControllerCamera : MonoBehaviour
{
    [SerializeField] GameObject[] cameraPositionCriterion;
    //[SerializeField] Animator cameraSwapAnimator;
    //[SerializeField] CinemachineCamera cineCam;
    int cameraPosIndex = 0;
    int cameraPosIndexMax;

    void Start()
    {
        cameraPosIndexMax = cameraPositionCriterion.Length - 1;
        if(cameraPosIndexMax == -1)
        {
            Debug.LogError("Shibal Assign at least one camera on coordinate");
        }

        //InitializeCameraAll();
    }

    void InitializeCameraAll()
    {
        for(int i = 0; i < cameraPosIndexMax; i++)
        {
            cameraPositionCriterion[i].SetActive(false);
        }
    }

    // Update is called once per frame
    internal void SwapCameraPos()
    {
        cameraPositionCriterion[cameraPosIndex].GetComponent<CinemachineCamera>().Priority = 1;
        if(cameraPosIndex == cameraPosIndexMax)
        {
            cameraPosIndex = 0;
        }
        else
        {
            cameraPosIndex++;
        }
        cameraPositionCriterion[cameraPosIndex].GetComponent<CinemachineCamera>().Priority = 10;
        
        Debug.Log("Changed camera pos as: " + cameraPositionCriterion[cameraPosIndex]);
        //cineCam.Follow = cameraPositionCriterion[cameraPosIndex].transform;
    }
}
