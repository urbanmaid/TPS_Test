using System.Collections;
using UnityEngine;

public class PlayerControllerStatus : MonoBehaviour
{
    [Header("Status Main")]
    internal int healthInit = 200; // 0 for death, 1 for alive, 2 for immovilized
    public int health = 1; // 0 for death, 1 for alive, 2 for immovilized
    IPlayerController playerController;
    private float deadStandbySeconds = 2.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void InitializeMe(IPlayerController maneuverOfParent)
    {
        playerController = maneuverOfParent;
        health = healthInit;
    }

    // Update is called once per frame
    public void SetDamage(int damageMagnitude)
    {
        if(health > 0)
        {
            health -= damageMagnitude;
            StartCoroutine(playerController.Camera.ShakeCamera(GameManager.instance.mainCamera.ActiveVirtualCamera));
        }
        
        if(health <= 0 && (playerController.ManeuverStatus != 0))
        {
            StartCoroutine(InvokeDead());
        }
    }

    public IEnumerator InvokeDead()
    {
        // Set you are dead and set camera as death Cam
        playerController.ManeuverStatus = 0;
        playerController.Camera.SetDeathCam(5f);

        yield return new WaitForSeconds(deadStandbySeconds);

        ResetPlayer();
    }

    public IEnumerator InvokeDeadAsFall()
    {
        // Set you are dead and set camera as death Cam
        playerController.ManeuverStatus = 0;
        health = 0;
        playerController.Camera.SetDeathCam(0f);

        yield return new WaitForSeconds(deadStandbySeconds);

        ResetPlayer();
    }

    void ResetPlayer()
    {
        // Remove death cam and let you respawn
        Debug.LogError("Respawning");
        playerController.Camera.ResetDeathCam();
        //playerControllerBase.Respawn();

        health = healthInit;
    }
}
