using System;
using UnityEngine;

public class PlayerControllerAnimation : MonoBehaviour
{
    private IPlayerController playerController;
    [SerializeField] GameObject playerAvatar;
    [SerializeField] Animator animator;

    bool isAbleToWork = true;

    internal void InitializeMe(IPlayerController iPlayerController)
    {
        playerController = iPlayerController;

        if(playerAvatar == null) isAbleToWork = false;

        playerController.OnManeuverStateChanged += HandleManeuverStateChange;
    }

    // Set direction of avatar
    internal void SetDirection()
    {
        if(isAbleToWork)
        {
            playerAvatar.transform.rotation = Quaternion.Euler(0f, playerController.LookDirectionAvatarX, 0f);

            /*
            if(playerController.IsAiming)
            {
                // If in aim state, player avatar follows the lookDirectionX Completely
                playerAvatar.transform.rotation = Quaternion.Euler(0f, playerController.LookDirectionX, 0f);
            }
            else
            {
                // Set Direction Offset with Move Direction and add with current lookDirectionX
                playerAvatar.transform.rotation = Quaternion.Euler(0f, playerController.LookDirectionAvatarX, 0f);
            }
            */
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
