using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider; // Reference to UI Slider
    [SerializeField] private AudioSource backgroundMusic; // Reference to Audio Source
    private const string BGM_VOLUME_KEY = "BGMVolume"; // PlayerPrefs key to save volume

    private void Start()
    {
        // Load saved volume or set default
        float savedVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 1f);
        bgmSlider.value = savedVolume;

        // Initialize background music volume
        if (backgroundMusic != null)
        {
            backgroundMusic.volume = savedVolume;
        }

        // Add listener for slider value change
        bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
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
        LoginManager.Instance.SignOut();
        Matchmaking.Instance.LeaveRoom();
    }
}
