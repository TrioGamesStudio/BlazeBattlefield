using Firebase.Extensions;
using Firebase.RemoteConfig;
using System;
using System.Threading.Tasks;
using UnityEngine;


[Serializable]
public class ConfigData {
    public string gameName;
    public float gameVersion;
    public bool over18;
}

public class RemoteConfig : MonoBehaviour
{
    public ConfigData allConfigData;

    private void Awake()
    {
        print("json:" + JsonUtility.ToJson(allConfigData));
        CheckRemoteConfigValues();
    }

    Task CheckRemoteConfigValues()
    {
        Debug.Log("Fetching data...");
        Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
        return fetchTask.ContinueWithOnMainThread(FetchComplete);
    }

    private void FetchComplete(Task fetchTask)
    {
        if (!fetchTask.IsCompleted)
        {
            Debug.LogError("Retrieval hasn't finished.");
            return;
        }

        var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
        var info = remoteConfig.Info;
        if (info.LastFetchStatus != LastFetchStatus.Success)
        {
            Debug.LogError($"{nameof(FetchComplete)} was unsuccessful\n{nameof(info.LastFetchStatus)}: {info.LastFetchStatus}");
            return;
        }

        // Fetch successful. Parameter values must be activated to use.
        Task a = remoteConfig.ActivateAsync();
        remoteConfig.ActivateAsync().ContinueWithOnMainThread(
            task => {
                Debug.Log($"Remote data loaded and ready for use. Last fetch time {info.FetchTime}.");

                string configData = remoteConfig.GetValue("All_Game_Data").StringValue;
                allConfigData = JsonUtility.FromJson<ConfigData>(configData);

                /* print("Total values: "+remoteConfig.AllValues.Count);

                foreach (var item in remoteConfig.AllValues)
                {
                    Debug.Log($"Key : {item.Key} | Value = {item.Value.StringValue}");
                } */

            });
    }

}
