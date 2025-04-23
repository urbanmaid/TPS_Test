using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] List<PlayerControllerManeuver> playerControllerManeuvers;
    public CinemachineBrain mainCamera;
    [SerializeField] GameObject deathCamObject;

    void Start()
    {
        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        /*
        foreach(PlayerControllerManeuver playerControllerManeuver in playerControllerManeuvers)
        {
            playerControllerManeuver.UpdateMe();
        }
        */
    }

    internal GameObject InstanciateDeathCam(Vector3 installPos, float distOfCamera)
    {
        Vector3 randomPosition = UnityEngine.Random.insideUnitSphere;
        randomPosition.y = 0; // y축을 고정 (필요에 따라 조정)
        return Instantiate(deathCamObject, installPos + (Vector3.up * 2.25f) + (randomPosition.normalized * distOfCamera), Quaternion.identity);
    }
}
