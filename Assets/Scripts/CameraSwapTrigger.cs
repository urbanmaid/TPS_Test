using Unity.Cinemachine;
using UnityEngine;

public class CameraSwapTrigger : MonoBehaviour
{
    [SerializeField] ICinemachineCamera cameraRegistered;
    [SerializeField] CinemachineCamera cameraTargetOfSwap;
    [SerializeField] CinemachineBrain cinemachineBrain;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Will be replaced via GameManager
        cameraTargetOfSwap.Follow = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("Currently using: " + cinemachineBrain.ActiveVirtualCamera);
            cameraRegistered = cinemachineBrain.ActiveVirtualCamera;

            if(cameraTargetOfSwap != null && cameraRegistered != null)
            {
                cameraTargetOfSwap.Priority = 12;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            cameraRegistered = null;

            if(cameraTargetOfSwap != null)
            {
                cameraTargetOfSwap.Priority = 0;
            }
        }
    } 
}
