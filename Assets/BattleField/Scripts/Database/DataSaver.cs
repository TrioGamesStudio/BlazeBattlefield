using System.Collections;
using UnityEngine;
using System;
using Firebase.Database;
using UnityEngine.SceneManagement;
using NaughtyAttributes;
using System.Collections.Generic;

[Serializable]
public class DataToSave
{
    public string userName;
    public int currLevel;
    public int winSolo;
    public int winTeam;
    public int coins;
    public int experience = 0; // Tracks total XP
    public int rank = 0;       // Tracks current rank
    public int totalPlaySolo;
    public int totalPlayTeam;

    public DataToSave()
    {

    }
    public DataToSave(string userName, int currLevel, int winSolo, int winTeam, int coins, int totalPlaySolo, int totalPlayTeam)
    {
        this.userName = userName;
        this.currLevel = currLevel;
        this.winSolo = winSolo;
        this.winTeam = winTeam;
        this.coins = coins;
        this.totalPlaySolo = totalPlaySolo;
        this.totalPlayTeam = totalPlayTeam;
    }
}

[Serializable]
public class InventoryDataToSave
{
    [Serializable] 
    public class CustomData
    {
        public CustomData(string _collectionsName, List<string> _collectionsList)
        {
            collectionsName = _collectionsName;
            collectionsList = _collectionsList;
        }
        public string collectionsName;
        public List<string> collectionsList;
    }
    public string inventoryName;
    //public string[] skinsArr = new string[5];
    //public List<string> skinsLists = new List<string>();
    public List<CustomData> CustomDatas = new();
    public InventoryDataToSave(string inventoryName, SkinDataHandler skinData, SkinDataHandler hatData)
    {
        this.inventoryName = inventoryName;
        //this.skinsArr = skinsNames;
        //this.skinsLists = lists;
        Debug.Log("Add Skin Collection name: " + skinData.CollectionsName);
        Debug.Log("Add Skin Collection name: " + hatData.CollectionsName);

        InitWhenLoad(skinData, hatData);

        
    }

    public void SaveSkinData(string collectionsName, List<string> collections)
    {
        foreach(var item in CustomDatas)
        {
            if(item.collectionsName == collectionsName)
            {
                item.collectionsList = collections;
            }
        }
    }

    public List<string> GetCollections(string collectionsName)
    {
        Debug.Log("Get Skin Collections name: " + collectionsName);
     
        foreach(var item in CustomDatas)
        {
            if(item.collectionsName == collectionsName)
            {
                return item.collectionsList;
            }
        }
        return new();
    }

    public void InitWhenLoad(SkinDataHandler skinData, SkinDataHandler hatData)
    {
        List<SkinDataHandler> skinDataHandlers = new()
        {
            skinData, hatData
        };
        CustomDatas.Clear();
        foreach (var item in skinDataHandlers)
        {
            CustomDatas.Add(new CustomData(item.CollectionsName, item.GetDefautlSkinData()));
        }
    }
}

public class DataSaver : MonoBehaviour
{
    public static DataSaver Instance;
    public int defaultMoneyWhenSigup = 600;
    public string userId;
    public DataToSave dataToSave;
    public InventoryDataToSave inventoryDataToSave;
    public SkinDataHandler skinDataHandler;
    public HatDataHandler hatDataHandler;
    //others
    DatabaseReference dbRef;

    private void Awake()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        if (Instance != null && this.gameObject != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(this);

    }

    public DataToSave ReturnDataToSave(string username, int currLevel, int winSolo, int winTeam, int coins, int totalPlaySolo, int totalPlayTeam)
    {
        return new DataToSave(username, currLevel, winSolo, winTeam, coins, totalPlaySolo, totalPlayTeam);
    }

    public void SaveToSignup(string userName, string userId)
    {
        DataToSave saveDataToSignup = ReturnDataToSave(userName, 1, 0, 0, defaultMoneyWhenSigup, 0, 0);
        // chuyen dataToSave -> json
        string json = JsonUtility.ToJson(saveDataToSignup);

        // tao folder trong database realtime
        dbRef.Child("Users").Child(userId).SetRawJsonValueAsync(json);
    }

    public InventoryDataToSave ReturnInvDataToSave(string inventoryName, string[] skinsArr, List<string> lists)
    {
        return new InventoryDataToSave(inventoryName, skinDataHandler, hatDataHandler);
    }


    public void SaveInvetoryToSignup(string userId)
    {

        InventoryDataToSave inv = new InventoryDataToSave("inventory", skinDataHandler, hatDataHandler);

        //inv.SaveSkinData(skinDataHandler.CollectionsName, skinDataHandler.GetDefautlSkinData());
        //inv.SaveSkinData(hatDataHandler.CollectionsName, skinDataHandler.GetDefautlSkinData());
        string json = JsonUtility.ToJson(inv);
        dbRef.Child("Inventory").Child(userId).SetRawJsonValueAsync(json);
    }


    #region  SAVE LOAD FIREBASE

    //? Save progress data
    public void SaveData()
    {
        #if (UNITY_WEBGL)
            Debug.Log("This is WebGL");
            return;
        #endif

        // chuyen dataToSave -> json
        string json = JsonUtility.ToJson(dataToSave);


        // tao folder trong database realtime
        dbRef.Child("Users").Child(userId).SetRawJsonValueAsync(json);
    }

    public void LoadData()
    {
        #if (UNITY_WEBGL)
            Debug.Log("This is WebGL");
            return;
        #endif

        StartCoroutine(LoadDataCO());

        LoadInventoryData();
        if (SceneManager.GetActiveScene().name == "Login") return;
    }

    IEnumerator LoadDataCO()
    {
        var serverData = dbRef.Child("Users").Child(userId).GetValueAsync();
        yield return new WaitUntil(() => serverData.IsCompleted);

        Debug.Log($"load process complete");

        DataSnapshot snapshot = serverData.Result;
        string jsonData = snapshot.GetRawJsonValue();

        if (jsonData != null)
        {
            Debug.Log($"found jsonData");
            dataToSave = JsonUtility.FromJson<DataToSave>(jsonData);
        }
        else
        {
            Debug.Log("jsonData not found");
        }
    }

    //? Save iventory data
    [Button]
    public void SaveInventoryData()
    {
        string json = JsonUtility.ToJson(inventoryDataToSave);
        dbRef.Child("Inventory").Child(userId).SetRawJsonValueAsync(json);

    }

    [Button]
    public void LoadInventoryData()
    {
        Debug.Log($"_____co load inventory");
        StartCoroutine(LoadInventoryDataCO());

        if (SceneManager.GetActiveScene().name == "Login") return;
    }

    IEnumerator LoadInventoryDataCO()
    {
        var serverData = dbRef.Child("Inventory").Child(userId).GetValueAsync();
        yield return new WaitUntil(() => serverData.IsCompleted);
        DataSnapshot snapshot = serverData.Result;
        string jsonData = snapshot.GetRawJsonValue();
        if (jsonData != null)
        {
            Debug.Log($"found jsonData");
            inventoryDataToSave = JsonUtility.FromJson<InventoryDataToSave>(jsonData);
            //inventoryDataToSave.InitWhenLoad(skinDataHandler, hatDataHandler);
        }
        else
        {
            Debug.Log("jsonData not found");
        }
    }

    public void ResetData()
    {
        // Reset all data fields to default values
        dataToSave.userName = "";
        dataToSave.currLevel = 1;
        dataToSave.winSolo = 0;
        dataToSave.winTeam = 0;
        dataToSave.coins = 0;
        dataToSave.experience = 0;
        dataToSave.rank = 0;
        dataToSave.totalPlaySolo = 0;
        dataToSave.totalPlayTeam = 0;

        // Optionally save the reset data to Firebase
        //SaveData();

        Debug.Log("Player data has been reset.");
    }
    #endregion SAVE LOAD FIREBASE

}