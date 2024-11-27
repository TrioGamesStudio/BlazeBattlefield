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
    [SerializeField] TextMeshProUGUI rankName;
    [SerializeField] TextMeshProUGUI currentLevel;
    [SerializeField] TextMeshProUGUI winSolo;
    [SerializeField] TextMeshProUGUI winTeam;
    [SerializeField] TextMeshProUGUI coin;
    [SerializeField] TextMeshProUGUI experienceText;
    [SerializeField] Slider experienceSlider;
    [SerializeField] Image rankIcon; // For player rank icon
    [SerializeField] Sprite[] rankIcons; // Array of rank icons based on rank index

    // buttons
    [Header("       Buttons")]
    [SerializeField] Button saveButton;
    [SerializeField] Button loadButton;
    [SerializeField] Button gotoLobby;
    [SerializeField] Button quickPlay;


    const string MAINLOBBY = "MainLobby";
    const string WORLD1 = "Quang_Scene";

    private int currentRank;
    private bool isSetup = false;
    //ohters
    DataSaver _dataSaver;
    private void Awake()
    {
        _dataSaver = FindObjectOfType<DataSaver>();
        _dataSaver.LoadData();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainLobby")
        {
            //ebug.Log("MainLobby scene loaded. Restarting coroutine.");
            StartCoroutine(ShowPlayerDataCo(0.5f));
        }
    }

    private void Start()
    {
        //saveButton?.onClick.AddListener(SaveManualTest);
        //loadButton?.onClick.AddListener(LoadMaunalTest);
        //gotoLobby?.onClick.AddListener(GoToLobby);
        //quickPlay?.onClick.AddListener(GoToQickBattle);
        //ShowPlayerName();
        //Debug.Log("xxx Update player info");
        //StartCoroutine(ShowPlayerDataCo(0.5f));
        StartCoroutine(LoadPlayerDataCo(0.3f));
    }

    IEnumerator LoadPlayerDataCo(float time)
    {
        yield return new WaitForSeconds(time);
        currentRank = DataSaver.Instance.dataToSave.rank;
        Debug.Log("xxx " + currentRank);
    }

    IEnumerator ShowPlayerDataCo(float time)
    {
        yield return new WaitForSeconds(time);
        //Debug.Log("xxx Update exp" );
        UpdateExperienceUI();
        ShowPlayerName();
        UpdateRankUI();     
    }

    void SaveManualTest()
    {
        _dataSaver.SaveData();
    }

    void LoadMaunalTest()
    {
        _dataSaver.LoadData();
        StartCoroutine(ShowPlayerDataCo(0.5f));
    }

    private void GoToLobby()
    {
        StartCoroutine(LoadToMainLobby(0.5f));
    }

    IEnumerator LoadToMainLobby(float time)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadSceneAsync(MAINLOBBY);
    }

    public void GoToQickBattle()
    {
        StartCoroutine(LoadToQuickBattle(0.5f));
    }

    IEnumerator LoadToQuickBattle(float time)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadSceneAsync(WORLD1);
    }

    public void ShowInfo()
    {
        // Debug.Log($"_____show player info");
        //userName.text = "User name: " + DataSaver.Instance.dataToSave.userName;
        //currentLevel.text = "Current Level: " + DataSaver.Instance.dataToSave.currLevel.ToString();
        rankName.text = "Rank: " + RankSystem.GetRankName(DataSaver.Instance.dataToSave.rank);
        winSolo.text = "Win: " + DataSaver.Instance.dataToSave.winSolo.ToString();
        winTeam.text = "Win Team: " + DataSaver.Instance.dataToSave.winTeam.ToString();
        coin.text = "Coin: " + DataSaver.Instance.dataToSave.coins.ToString();
    }

    public void ShowPlayerName()
    {
        userName.text = DataSaver.Instance.dataToSave.userName;
    }

    void UpdateRankUI()
    {
        var playerData = DataSaver.Instance.dataToSave;

        // Update rank icon
        if (rankIcons != null && playerData.rank < rankIcons.Length)
        {
            rankIcon.sprite = rankIcons[playerData.rank];
        }
        else
        {
            Debug.LogWarning("Rank icon not found for rank: " + playerData.rank);
        }
    }

    void UpdateExperienceUI()
    {
        var playerData = DataSaver.Instance.dataToSave;

        // Update experience display
        float currentXP = playerData.experience;
        int updateRank = playerData.rank;
        float nextThreshold = RankSystem.GetNextThreshold(updateRank);
        if (updateRank != currentRank)
        {
            Debug.Log("xxx LEVEL UP");
            currentRank = updateRank;
            playerData.experience = 0;
            // save to firebase datatosave
            DataSaver.Instance.SaveData();
        }
        //Debug.Log("xxx " + currentXP + " save experince" + playerData.experience);
        currentXP = playerData.experience;
        experienceText.text = $"{currentXP}/{nextThreshold}exp";

        // Update experience slider
        if (experienceSlider != null)
        {
            experienceSlider.value = currentXP/nextThreshold;
        }
    }
}