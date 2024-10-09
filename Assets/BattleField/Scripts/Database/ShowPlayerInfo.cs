using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using UnityEditor.ShaderGraph.Internal;

public class ShowPlayerInfo : MonoBehaviour
{
    // show info UI
    [SerializeField] TextMeshProUGUI userName;
    [SerializeField] TextMeshProUGUI currentLevel;
    [SerializeField] TextMeshProUGUI coins;
    [SerializeField] TextMeshProUGUI highScore;

    // buttons
    [SerializeField] Button saveButton;
    [SerializeField] Button loadButton;
    [SerializeField] Button gotoLobby;



    //ohters
    DataSaver dataSaver;
    private void Awake() {
        dataSaver = FindObjectOfType<DataSaver>();
    }

    private void Start() {
        saveButton.onClick.AddListener(Save);
        loadButton.onClick.AddListener(Load);
        gotoLobby.onClick.AddListener(GoToLobby);
    }

    IEnumerator ShowPlayerDataCO() {
        yield return new WaitForSeconds(0.5f);
        ShowInfo();
    }
    void Save() {
        dataSaver.SaveData();
    }

    void Load() {
        dataSaver.LoadData();
        StartCoroutine(ShowPlayerDataCO());
    }
    private void GoToLobby()
    {
        StartCoroutine(LoadToMainLobby(0.5f));
    }

    IEnumerator LoadToMainLobby(float time) {
        yield return new WaitForSeconds(time);
        SceneManager.LoadSceneAsync("MainLobby");
    }
    void ShowInfo() {
        Debug.Log($"_____show player info");
        userName.text = "User name: " + DataSaver.Instance.dataToSave.userName;
        currentLevel.text = "Current Level: " + DataSaver.Instance.dataToSave.currLevel.ToString();
        coins.text = "Coins: " + DataSaver.Instance.dataToSave.coins.ToString();
        highScore.text = "High Score: " + DataSaver.Instance.dataToSave.highScore.ToString();
    }
}
