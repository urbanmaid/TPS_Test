using Unity.Mathematics;
using UnityEngine;

public class Damagables : MonoBehaviour
{
    [SerializeField] int damageMagnitude = 40;
    [SerializeField] bool isFallRelated = false;
    // Update is called once per frame

    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Player"))
        {
            PlayerControllerStatus playerControllerStatus = col.GetComponent<PlayerControllerStatus>();
            if(playerControllerStatus)
            {
                if(isFallRelated)
                {
                    StartCoroutine(playerControllerStatus.InvokeDeadAsFall());
                }
                else
                {
                    playerControllerStatus.SetDamage(damageMagnitude);
                }
            }
        }
    }
}
