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
    [SerializeField] TextMeshProUGUI winSolo;
    [SerializeField] TextMeshProUGUI winTeam;
    [SerializeField] TextMeshProUGUI coin;

    // buttons
    [Header("       Buttons")]
    [SerializeField] Button saveButton;
    [SerializeField] Button loadButton;
    [SerializeField] Button gotoLobby;
    [SerializeField] Button quickPlay;


    const string MAINLOBBY = "MainLobby";
    const string WORLD1 = "World1";

    //ohters
    DataSaver _dataSaver;
    private void Awake() {
        _dataSaver = FindObjectOfType<DataSaver>();
    }

    private void Start() {
        saveButton.onClick.AddListener(SaveManualTest);
        loadButton.onClick.AddListener(LoadMaunalTest);
        gotoLobby.onClick.AddListener(GoToLobby);
        quickPlay.onClick.AddListener(GoToQickBattle);


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
        SceneManager.LoadSceneAsync(MAINLOBBY);
    }

    private void GoToQickBattle()
    {
        StartCoroutine(LoadToQuickBattle(0.5f));
    }

    IEnumerator LoadToQuickBattle(float time) {
        yield return new WaitForSeconds(time);
        SceneManager.LoadSceneAsync(WORLD1);
    }
    
    void ShowInfo() {
        // Debug.Log($"_____show player info");
        userName.text = "User name: " + DataSaver.Instance.dataToSave.userName;
        currentLevel.text = "Current Level: " + DataSaver.Instance.dataToSave.currLevel.ToString();
        winSolo.text = "Win: " + DataSaver.Instance.dataToSave.winSolo.ToString();
        winTeam.text = "Win Team: " + DataSaver.Instance.dataToSave.winTeam.ToString();
        coin.text = "Coin: " + DataSaver.Instance.dataToSave.coins.ToString();
    }
}
