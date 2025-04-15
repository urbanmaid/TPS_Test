using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIIsConnectingToggle : MonoBehaviour
{
    private Toggle _toggle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _toggle = gameObject.GetComponent<Toggle>();

        if (_toggle != null)
        {
            _toggle.onValueChanged.AddListener(OnValueChanged);
        }
        else
        {
            Debug.LogWarning("No TMP_InputField is available!");
        }
    }

    // Update is called once per frame
    void OnValueChanged(bool value)
    {
        NetworkRunnerManager.instance.isUsingNetwork = value;
    }
}
