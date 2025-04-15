using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIPlayButton : MonoBehaviour
{
    Button _btn;

    void Start()
    {
        _btn = GetComponent<Button>();
        _btn.onClick.AddListener(OnClick);
    }

    // Update is called once per frame
    void OnClick()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
