using TMPro;
using UnityEngine;

public class UIConnectedSessionInput : MonoBehaviour
{
    private TMP_InputField _inputField;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _inputField = gameObject.GetComponent<TMP_InputField>();

        if (_inputField != null)
        {
            _inputField.onEndEdit.AddListener(OnInputValueChanged);
        }
        else
        {
            Debug.LogWarning("No TMP_InputField is available!");
        }
    }

    // Update is called once per frame
    void OnInputValueChanged(string input)
    {
        NetworkRunnerManager.instance.userSessionName = input;
    }
}
