using UnityEngine;

public class InGameScene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(NetworkRunnerManager.instance)
        {
            NetworkRunnerManager.instance.StartConnection();
        }
        else
        {
            Debug.LogWarning("Running On Local Mode");
        }
    }
}
