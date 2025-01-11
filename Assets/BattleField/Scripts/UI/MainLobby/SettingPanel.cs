using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider; // Reference to UI Slider
    [SerializeField] private AudioSource backgroundMusic; // Reference to Audio Source
    [SerializeField] private string backgroundMusicTag = "BackgroundMusic";
    private const string BGM_VOLUME_KEY = "BGMVolume"; // PlayerPrefs key to save volume
    public static Action OnLogoutEvent;
    private void Start()
    {
        // Load saved volume or set default
        float savedVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 1f);
        bgmSlider.value = savedVolume;

        // Set up initial audio source reference
        FindAndSetupAudioSource();

        // Initialize background music volume
        if (backgroundMusic != null)
        {
            backgroundMusic.volume = savedVolume;
        }

        // Add listener for slider value change
        bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Re-find and setup audio source when a new scene is loaded
        FindAndSetupAudioSource();
    }

    private void FindAndSetupAudioSource()
    {
        // Find the audio source by tag
        GameObject musicObject = GameObject.FindWithTag(backgroundMusicTag);
        if (musicObject != null)
        {
            backgroundMusic = musicObject.GetComponent<AudioSource>();
            if (backgroundMusic != null)
            {
                backgroundMusic.volume = bgmSlider.value;
            }
        }
    }

    private void OnBGMVolumeChanged(float volume)
    {
        // Update background music volume
        if (backgroundMusic != null)
        {
            backgroundMusic.volume = volume;
        }

        // Save volume setting
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }

    public void SignOut()
    {
        #if UNITY_WEBGL || UNITY_IOS
            Debug.Log("===This is webgl");
            SceneManager.LoadSceneAsync("Start");
            UIController.Instance.ShowHideUI(UIController.Instance.mainLobbyPanel);
            Matchmaking.Instance.LeaveRoom();
            OnLogoutEvent?.Invoke();
            return;
        #else
            LoginManager.Instance.SignOut();
            Matchmaking.Instance.LeaveRoom();
            UIController.Instance.ShowHideUI(UIController.Instance.mainLobbyPanel);
            OnLogoutEvent?.Invoke();
        #endif
    }

    private void OnDestroy()
    {
        // Remove all listeners
        if (bgmSlider != null)
        {
            bgmSlider.onValueChanged.RemoveListener(OnBGMVolumeChanged);
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
