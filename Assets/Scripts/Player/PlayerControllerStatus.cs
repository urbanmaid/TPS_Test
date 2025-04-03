using System.Collections;
using UnityEngine;

public class PlayerControllerStatus : MonoBehaviour
{
    [Header("Status Main")]
    internal int healthInit = 200; // 0 for death, 1 for alive, 2 for immovilized
    [SerializeField] int health = 1; // 0 for death, 1 for alive, 2 for immovilized
    PlayerControllerManeuver maneuver;
    private float deadStandbySeconds = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void InitializeMe(PlayerControllerManeuver maneuverOfParent)
    {
        maneuver = maneuverOfParent;
        health = healthInit;
    }

    // Update is called once per frame
    public void SetDamage(int damageMagnitude)
    {
        if(health > 0)
        {
            health -= damageMagnitude;
            StartCoroutine(maneuver.playerControllerCamera.ShakeCamera(GameManager.instance.mainCamera.ActiveVirtualCamera));
        }
        
        if(health <= 0 && (maneuver.maneuverStatus != 0))
        {
            StartCoroutine(InvokeDead());
        }
    }

    public IEnumerator InvokeDead()
    {
        // Set you are dead and set camera as death Cam
        Debug.LogError("You are dead.");
        maneuver.maneuverStatus = 0;
        maneuver.playerControllerCamera.SetDeathCam();

        yield return new WaitForSeconds(deadStandbySeconds);

        // Remove death cam and let you respawn
        Debug.LogError("Respawning");
        maneuver.playerControllerCamera.ResetDeathCam();
        maneuver.Respawn();

        health = healthInit;
    }
}
