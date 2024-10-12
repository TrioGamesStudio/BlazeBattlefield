using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//gameobject = 
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
    DataSaver _dataSaver;
    private void Awake() {
        _dataSaver = FindObjectOfType<DataSaver>();
    }

    private void Start() {
        saveButton.onClick.AddListener(SaveManualTest);
        loadButton.onClick.AddListener(LoadMaunalTest);
        gotoLobby.onClick.AddListener(GoToLobby);

        StartCoroutine(ShowPlayerDataCo(0.5f));
    }

    IEnumerator ShowPlayerDataCo(float time) {
        yield return new WaitForSeconds(time);
        ShowInfo();
        StopAllCoroutines();
    }

    void SaveManualTest() {
        _dataSaver.SaveData();
    }

    void LoadMaunalTest() {
        _dataSaver.LoadData();
        StartCoroutine(ShowPlayerDataCo(0.5f));
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
        // Debug.Log($"_____show player info");
        userName.text = "User name: " + DataSaver.Instance.dataToSave.userName;
        currentLevel.text = "Current Level: " + DataSaver.Instance.dataToSave.currLevel.ToString();
        highScore.text = "High Score: " + DataSaver.Instance.dataToSave.highScore.ToString();
        coins.text = "Coins: " + DataSaver.Instance.dataToSave.coins.ToString();
    }
}
