using System.Collections;
using UnityEngine;
using System;
using Firebase.Database;
using UnityEngine.SceneManagement;

[Serializable]
public class DataToSave {
    public string userName;
    public int currLevel;
    public int winSolo;
    public int winTeam;
    public int coins;
    public int experience = 0; // Tracks total XP
    public int rank = 0;       // Tracks current rank

    public DataToSave() {
        
    }
    public DataToSave(string userName, int currLevel, int winSolo, int winTeam, int coins) {
        this.userName = userName;
        this.currLevel = currLevel;
        this.winSolo = winSolo;
        this.winTeam = winTeam;
        this.coins = coins;
    }
}

public class DataSaver : MonoBehaviour
{
    public static DataSaver Instance;

    public string userId;
    public DataToSave dataToSave;

    //others
    DatabaseReference dbRef;

    // buttons
    /* [SerializeField] Button saveButton;
    [SerializeField] Button loadButton; */

    private void Awake() {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        if(Instance != null && this.gameObject != null) {
            Destroy(this.gameObject);
        }
        else {
            Instance = this;
        }
    }

    private void Start() {
        /* saveButton.onClick.AddListener(SaveData);
        loadButton.onClick.AddListener(LoadData); */

        DontDestroyOnLoad(this);
        
    }

    public DataToSave ReturnDataToSave(string username, int currLevel, int winSolo,int winTeam, int coins) {
        return new DataToSave(username, currLevel, winSolo, winTeam, coins);
    }

    public void SaveToSignup(string userName, string userId) {
        DataToSave saveDataToSignup = ReturnDataToSave(userName, 1, 0, 0, 0);
        // chuyen dataToSave -> json
        string json = JsonUtility.ToJson(saveDataToSignup);

        // tao folder trong database realtime
        dbRef.Child("Users").Child(userId).SetRawJsonValueAsync(json);
    }

    #region  SAVE LOAD FIREBASE
    public void SaveData() {
        // chuyen dataToSave -> json
        string json = JsonUtility.ToJson(dataToSave);

        // tao folder trong database realtime
        dbRef.Child("Users").Child(userId).SetRawJsonValueAsync(json);
    }

    public void LoadData() {
        StartCoroutine(LoadDataCO());

        if(SceneManager.GetActiveScene().name == "Login") return;
    }

    IEnumerator LoadDataCO() {
        var serverData = dbRef.Child("Users").Child(userId).GetValueAsync();
        yield return new WaitUntil(() => serverData.IsCompleted);

        Debug.Log($"load process complete");

        DataSnapshot snapshot = serverData.Result;
        string jsonData = snapshot.GetRawJsonValue();

        if(jsonData != null) {
            Debug.Log($"found jsonData");
            dataToSave = JsonUtility.FromJson<DataToSave>(jsonData);
        }
        else {
            Debug.Log("jsonData not found");
        }
    }
    #endregion SAVE LOAD FIREBASE

}