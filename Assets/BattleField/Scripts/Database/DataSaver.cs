using System.Collections;
using UnityEngine;
using System;
using Firebase.Database;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[Serializable]
public class DataToSave {
    public string userName;
    public int currLevel;
    public int coins;
    public int highScore;
    
    public DataToSave() {}
    public DataToSave(string userName, int coins, int currLevel, int highScore) {
        this.userName = userName;
        this.coins = coins;
        this.currLevel = currLevel;
        this.highScore = highScore;
    }
}

public class DataSaver : MonoBehaviour
{
    public static DataSaver Instance;

    public string userId;
    public DataToSave dataToSave;
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

    public DataToSave ReturnDataToSave(string username, int coins, int currLevel, int hightScore) {
        return new DataToSave(username, coins, currLevel, hightScore);
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
        //ShowInfo();
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