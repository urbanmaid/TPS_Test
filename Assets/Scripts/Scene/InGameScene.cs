using UnityEngine;

public class InGameScene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        NetworkRunnerManager.instance.StartConnection();
    }
}
