using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class InputName : MonoBehaviour
{
    public TMP_InputField nameInputField;
    [SerializeField] Button playButton;
    const string MAINLOBBY = "MainLobby";
    void Start()
    {
        playButton.onClick.AddListener(SetName);
        nameInputField.text = GameManager.names[Random.Range(0, GameManager.names.Length)];
    }

    public void SetName()
    {
        StartCoroutine(LoadToMainLobby(0.5f));
    }

    IEnumerator LoadToMainLobby(float time) {
        GameManager.playerNickName = nameInputField.text;
        DataSaver.Instance.dataToSave.userName = nameInputField.text;
        yield return new WaitForSeconds(time);
        SceneManager.LoadSceneAsync(MAINLOBBY);
    }
}
