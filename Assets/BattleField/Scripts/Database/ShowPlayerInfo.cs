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

    // show player stats
    [SerializeField] TextMeshProUGUI userNameStat;
    [SerializeField] TextMeshProUGUI rankNameStat;
    [SerializeField] TextMeshProUGUI winSoloStat;
    [SerializeField] TextMeshProUGUI winTeamStat;
    [SerializeField] TextMeshProUGUI totalPlaySoloStat;
    [SerializeField] TextMeshProUGUI totalPlayTeamStat;
    [SerializeField] TextMeshProUGUI winRateSoloStat;
    [SerializeField] TextMeshProUGUI winRateTeamStat;

    // buttons
    [Header("       Buttons")]
    [SerializeField] Button saveButton;
    [SerializeField] Button loadButton;
    [SerializeField] Button gotoLobby;
    [SerializeField] Button quickPlay;


    const string MAINLOBBY = "MainLobby";
    const string WORLD1 = "Quang_Scene";

    public int currentRank;
    private bool isLeveUp = false;
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
            Debug.Log("xxx MainLobby scene loaded. Restarting coroutine show player info.");     
            StartCoroutine(ShowPlayerDataCo(0.3f));
        }
    }

    private void Start()
    {
        Debug.Log("xxx MainLobby scene loaded. Restarting coroutine load player info.");
        StartCoroutine(LoadPlayerDataCo(0.1f));
    }

    public IEnumerator LoadPlayerDataCo(float time)
    {
        yield return new WaitForSeconds(time);
        currentRank = DataSaver.Instance.dataToSave.rank;
        Debug.Log("xxx " + currentRank);
    }

    public IEnumerator ShowPlayerDataCo(float time)
    {
        yield return new WaitForSeconds(time);
        Debug.Log("xxx Reset Player info UI" );
        UpdateExperienceUI();
        ShowPlayerName();
        UpdateRankUI();     
    }

    public void ShowPlayerData()
    {
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
        StartCoroutine(LoadToMainLobby(0.1f));
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
        rankName.text = "Rank: " + RankSystem.GetRankName(DataSaver.Instance.dataToSave.rank);
        winSolo.text = "Win: " + DataSaver.Instance.dataToSave.winSolo.ToString();
        winTeam.text = "Win Team: " + DataSaver.Instance.dataToSave.winTeam.ToString();
        coin.text = "Coin: " + DataSaver.Instance.dataToSave.coins.ToString();
    }

    public void ShowInfoStat()
    {
        userNameStat.text = DataSaver.Instance.dataToSave.userName;
        rankNameStat.text = RankSystem.GetRankName(DataSaver.Instance.dataToSave.rank);
        winSoloStat.text = DataSaver.Instance.dataToSave.winSolo.ToString();
        winTeamStat.text = DataSaver.Instance.dataToSave.winTeam.ToString();
        totalPlaySoloStat.text = DataSaver.Instance.dataToSave.totalPlaySolo.ToString();
        totalPlayTeamStat.text = DataSaver.Instance.dataToSave.totalPlayTeam.ToString();
        if (DataSaver.Instance.dataToSave.winSolo != 0 && DataSaver.Instance.dataToSave.totalPlaySolo != 0)
        {
            winRateSoloStat.text = ((float)DataSaver.Instance.dataToSave.winSolo / DataSaver.Instance.dataToSave.totalPlaySolo * 100f).ToString("F1") + " %";
            winRateTeamStat.text = ((float)DataSaver.Instance.dataToSave.winTeam / DataSaver.Instance.dataToSave.totalPlayTeam * 100f).ToString("F1") + " %";
        }   
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
        if (currentRank != updateRank)
        {
            Debug.Log("xxx LEVEL UP");
            FindObjectOfType<LevelUpPanel>().ShowLevelUpPanel(updateRank - 1, updateRank);
            currentRank = updateRank;
            isLeveUp = false;
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

    public void ResetRank()
    {
        currentRank = 0;
    }

    public void SetLevelUp()
    {
        isLeveUp = true;
    }
}