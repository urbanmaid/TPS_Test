using System;
using UnityEngine;

public class PlayerControllerAnimation : MonoBehaviour
{
    private PlayerControllerManeuver maneuver;
    [SerializeField] GameObject playerAvatar;
    [SerializeField] Animator animator;

    bool isAbleToWork = true;

    internal void InitializeMe(PlayerControllerManeuver playerControllerManeuver)
    {
        maneuver = playerControllerManeuver;

        if(playerAvatar == null) isAbleToWork = false;

        maneuver.OnManeuverStateChanged += HandleManeuverStateChange;
    }

    // Set direction of avatar
    internal void SetDirection(float lookDirectionX)
    {
        if(isAbleToWork)
        {
            playerAvatar.transform.rotation = Quaternion.Euler(0f, lookDirectionX, 0f);
        }
    }

    // Set animation act of avatar
    private void HandleManeuverStateChange(int status, bool aiming)
    {
        if (!isAbleToWork) return;

        // maneuverStatus에 따른 동작
        switch (status)
        {
            case 0:
                Debug.Log("Player Animation: Death");
                break;
            case 1:
                Debug.Log("Player Animation: Alive");
                break;
            case 2:
                Debug.Log("Player Animation: Sliding");
                break;
            case 3:
                Debug.Log("Player Animation: Jumping");
                break;
            default:
                Debug.LogWarning($"Unknown maneuver status: {status}");
                break;
        }

        // 조준 상태 반영
        if (aiming)
        {
            Debug.Log("Player Animation: Aiming");
        }
        else
        {
            Debug.Log("Player Animation: Not Aiming");
        }
    }

    // Update is called once per frame
}
