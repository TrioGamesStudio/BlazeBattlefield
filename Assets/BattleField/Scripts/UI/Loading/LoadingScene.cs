using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Fusion;

public class LoadingScene : MonoBehaviour
{
    public static LoadingScene Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject loadingCanvas;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private TextMeshProUGUI tipText;

    [Header("Loading Settings")]
    [SerializeField] private float minimumLoadTime = 3f;
    [SerializeField] private string[] loadingTips;

    private AsyncOperation sceneLoadOperation;
    private float loadStartTime;
    private bool isLoading = false;

    private NetworkRunner activeRunner;

    private void Awake()
    {
        // Singleton pattern setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupLoadingScreen();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SetupLoadingScreen()
    {
        // Initially hide the loading screen
        loadingCanvas.gameObject.SetActive(false);

        // Ensure loading screen appears on top of everything
        //loadingCanvas.sortingOrder = 999;
    }

    // Public method to load a scene by name
    public void LoadScene(string sceneName)
    {
        if (!isLoading)
        {
            StartCoroutine(LoadSceneRoutine(sceneName));
        }
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        isLoading = true;
        loadStartTime = Time.time;
        loadingCanvas.gameObject.SetActive(true);

        // Reset UI elements
        progressBar.value = 0f;
        if (loadingTips != null && loadingTips.Length > 0)
        {
            tipText.text = loadingTips[Random.Range(0, loadingTips.Length)];
        }

        // Start loading the scene asynchronously
        sceneLoadOperation = SceneManager.LoadSceneAsync(sceneName);
        sceneLoadOperation.allowSceneActivation = false;

        yield return StartCoroutine(HandleLoading());
    }

    private IEnumerator HandleLoading()
    {
        while (!IsLoadingComplete1())
        {
            float progress = Mathf.Clamp01(sceneLoadOperation.progress / 0.9f);
            UpdateLoadingUI1(progress);
            yield return null;
        }

        // Loading is complete
        sceneLoadOperation.allowSceneActivation = true;

        // Wait for the scene to activate
        yield return new WaitForSeconds(0.1f);

        // Hide the loading screen
        loadingCanvas.gameObject.SetActive(false);

        //UIController.Instance.ShowHideUI(UIController.Instance.mainLobbyPanel);
        UIController.Instance.ShowHidePanel(UIController.Instance.mainLobbyPanel);
        isLoading = false;
    }

    private bool IsLoadingComplete1()
    {
        float elapsedTime = Time.time - loadStartTime;
        return elapsedTime >= minimumLoadTime && sceneLoadOperation.progress >= 0.9f;
    }

    private void UpdateLoadingUI1(float progress)
    {
        progressBar.value = progress;
        int percentComplete = Mathf.RoundToInt(progress * 100);
        progressText.text = $"{percentComplete}%";
    }

    public void ShowLoadingScreen(NetworkRunner runner)
    {
        if (!isLoading)
        {
            StartCoroutine(ShowLoadingScreenRoutine(runner));
        }
    }

    private IEnumerator ShowLoadingScreenRoutine(NetworkRunner runner)
    {
        isLoading = true;
        activeRunner = runner;
        loadStartTime = Time.time;

        // Show loading screen and reset UI
        loadingCanvas.gameObject.SetActive(true);
        progressBar.value = 0f;

        // Show random tip
        if (loadingTips != null && loadingTips.Length > 0)
        {
            tipText.text = loadingTips[Random.Range(0, loadingTips.Length)];
        }

        // Monitor loading progress
        while (!IsLoadingComplete())
        {
            if (activeRunner != null)
            {
                float progress = CalculateLoadingProgress();
                UpdateLoadingUI(progress);
            }
            yield return null;
        }

        // Add a small delay before hiding
        yield return new WaitForSeconds(0.5f);

        // Hide loading screen
        loadingCanvas.gameObject.SetActive(false);
        isLoading = false;
        activeRunner = null;
    }

    private float CalculateLoadingProgress()
    {
        if (activeRunner == null) return 0f;

        // Get progress from Fusion's network runner
        float networkProgress = activeRunner.IsRunning ? 1f : 0.5f;
        float timeProgress = Mathf.Clamp01((Time.time - loadStartTime) / minimumLoadTime);

        // Combine both progress values
        return Mathf.Min(networkProgress, timeProgress);
    }

    private bool IsLoadingComplete()
    {
        if (activeRunner == null) return true;

        float elapsedTime = Time.time - loadStartTime;
        return elapsedTime >= minimumLoadTime && activeRunner.IsRunning;
    }

    private void UpdateLoadingUI(float progress)
    {
        progressBar.value = progress;
        int percentComplete = Mathf.RoundToInt(progress * 100);
        progressText.text = $"{percentComplete}%";
    }

    public void HideLoadingScreen()
    {
        StopAllCoroutines();
        loadingCanvas.gameObject.SetActive(false);
        isLoading = false;
        activeRunner = null;
    }
}
