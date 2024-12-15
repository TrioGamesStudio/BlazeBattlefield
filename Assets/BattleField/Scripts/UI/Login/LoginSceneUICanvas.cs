using UnityEngine;
using UnityEngine.UI;

public class LoginSceneUICanvas : MonoBehaviour
{
    [SerializeField] Button quitButton;

    void Start()
    {
        quitButton.onClick.AddListener(() => {
            Debug.Log($"____Quit");
            Application.Quit();
        });
    }

    void QuitGame() {
        Application.Quit();
    }
}
