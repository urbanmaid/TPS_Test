using Unity.Mathematics;
using UnityEngine;

public class Damagables : MonoBehaviour
{
    [SerializeField] int damageMagnitude = 40;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * 100f);
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Player"))
        {
            PlayerControllerStatus playerControllerStatus = col.GetComponent<PlayerControllerStatus>();
            if(playerControllerStatus)
            {
                playerControllerStatus.SetDamage(damageMagnitude);
            } 
        }
    }
}
