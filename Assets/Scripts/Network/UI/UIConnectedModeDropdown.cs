using TMPro;
using UnityEngine;

public class UIConnectedModeDropdown : MonoBehaviour
{
    private TMP_Dropdown _dropdown;

    void Start()
    {
        _dropdown = gameObject.GetComponent<TMP_Dropdown>();

        if (_dropdown != null)
        {
            _dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        }
        else
        {
            Debug.LogWarning("No TMP_InputField is available!");
        }
    }

    void OnDropdownValueChanged(int index)
    {
        NetworkRunnerManager.instance.userGameMode = (Fusion.GameMode)(index + 4);
    }
}
