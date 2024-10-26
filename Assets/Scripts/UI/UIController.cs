using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class UIController : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject panelMode;        
    public GameObject panelTeamJoin;
    public Button showButton;       
    public GameObject background;
    public Image modeImage;
    public Sprite singleMode;
    public Sprite duoMode;
    public ModeButton soloButton;
    public ModeButton duoButton;
    public Toggle toggleModeButton;
    public Button joinTeamButton;
    //public TextMeshProUGUI informationText;
    public GameObject mainLobbyPanel;
    public GameObject worldPanel;
    private bool isPanelActive = false;
    public GameObject sessionButtonPrefab;  // Prefab to represent a session in UI
    public Transform sessionListContent;    // Parent transform for session buttons
    public GameObject panelResult;
    public Button lobbyButton;
    //bool isSingle = true;
    public static UIController Instance { get; private set; }
    private Matchmaking matchmaking;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate
        }
        matchmaking = FindObjectOfType<Matchmaking>();
    }

    void Start()
    {
        InitializeUI();
        toggleModeButton.onValueChanged.AddListener(OnAutoMatchToggleChanged);
        joinTeamButton.onClick.AddListener(() => ShowHidePanel(panelTeamJoin));
        toggleModeButton.interactable = false;
        //lobbyButton.onClick.AddListener(BackToLobby);
    }

    void Update()
    {
        //if (SceneManager.GetActiveScene().name != "MainLobby") return;
        HandleTouchInput();
    }

    // Method to handle the Toggle UI value change
    private void OnAutoMatchToggleChanged(bool isOn)
    {
        matchmaking.IsAutoMatch = isOn;
        Debug.Log("isAutoMatch set to: " + isOn);
    }

    // Initializes the button listeners and ensures the panel is hidden initially
    private void InitializeUI()
    {
        HidePanel();

        if (showButton != null)
        {
            // Assign the button's onClick event to toggle the panel
            showButton.onClick.AddListener(ShowPanel);
        }
    }

    // Handles touch input to hide the panel if the touch is on the background but not on the button
    private void HandleTouchInput()
    {
        bool isMainLobbyScene = SceneManager.GetActiveScene().name == "MainLobby";
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && isMainLobbyScene)
        {
            if (IsTouchOnBackgroundOnly())
            {
                HidePanel();
            }
        }
    }

    private bool IsTouchOnBackgroundOnly()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = Input.GetTouch(0).position;
        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        // Check if the only hit UI element is the background
        return results.Count == 1 && results[0].gameObject == background;

        //// Check if the touch is over any UI element
        //PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        //eventDataCurrentPosition.position = Input.GetTouch(0).position;
        //List<RaycastResult> results = new();
        //EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        //return results.Count > 0;
    }

    // Toggles the panel between show and hide states
    private void TogglePanel()
    {
        if (isPanelActive)
        {
            HidePanel();
        }
        else
        {
            ShowPanel();
        }
    }

    // Method to show the panel and set it active
    public void ShowPanel()
    {
        if (isPanelActive) return;
        panelMode.SetActive(true);
        isPanelActive = true;
    }

    // Method to hide the panel and set it inactive
    public void HidePanel()
    {
        panelMode.SetActive(false);
        panelTeamJoin.SetActive(false);
        isPanelActive = false;
    }

    public void SwitchMode(bool single)
    {
        if (single)
        {
            modeImage.sprite = singleMode;
            soloButton.Highlight();
            duoButton.UnHightLight();
            toggleModeButton.interactable = false;
        }        
        else
        {
            modeImage.sprite = duoMode;
            soloButton.UnHightLight();
            duoButton.Highlight();
            toggleModeButton.interactable = true;

        }
        //isSingle = !isSingle;
    }

    public void ShowHidePanel(GameObject panelUI)
    {
        if (panelUI.activeSelf == false)
        {
            panelUI.SetActive(true);
        }
    }

    public void ShowHideUI(GameObject panelUI)
    {
       panelUI.SetActive(!panelUI.activeSelf);
    }

    public void OnOffPanel()
    {
        joinTeamButton.interactable = !joinTeamButton.interactable;
    }

    public void CreateRoomUI(string roomname, int playerCount, int maxPlayer)
    {
        GameObject sessionButton = Instantiate(sessionButtonPrefab, sessionListContent);
        string infomation = $"{playerCount}/{maxPlayer}";
        sessionButton.GetComponent<RoomPanelUI>().SetRoomButtonUI(infomation, roomname);
        //bool isRoomFull = session.PlayerCount >= session.MaxPlayers;
        //sessionButton.GetComponent<RoomButtonUI>().Deactive(isRoomFull);
    }

    public void ClearSessionButtons()
    {
        // Iterate through all child objects (session buttons) of the sessionListContent and destroy them
        foreach (Transform child in sessionListContent)
        {
            Destroy(child.gameObject);
        }
    }

    public  void SetText(string text)
    {
        //informationText.text = text;
        FindObjectOfType<WorldUI>().SetText(text);
    }

    public void StartCountdown()
    {
        //StartCoroutine(CountdownCoroutine());
        FindObjectOfType<WorldUI>().StartCountdown();
    }

    //private IEnumerator CountdownCoroutine()
    //{
    //    int countdownValue = 3;  // Start from 3
    //    yield return new WaitForSeconds(1f);
    //    while (countdownValue > 0)
    //    {
    //        yield return new WaitForSeconds(1f);             // Wait for 1 second
    //        informationText.text = countdownValue.ToString();  // Update the countdown text
    //        //countdownText.gameObject.SetActive(true);        // Show the countdown UI        
    //        countdownValue--;
    //    }

    //    // Countdown finished
    //    informationText.text = "GO!";
    //    yield return new WaitForSeconds(1f);  // Keep "GO!" for 1 second

    //    // Hide the countdown UI
    //    informationText.gameObject.SetActive(false);

    //    // Load the battle scene
    //    //TransitionToBattleScene(runner);
    //}

    //private void BackToLobby()
    //{
    //    Debug.Log("BACK TO LOBBY NE");
    //}

    public void ShowResultPanel(int alivePlayer)
    {
        FindObjectOfType<WorldUI>().ShowHideUI(alivePlayer);
    }

    public void ResetUI()
    {
        SwitchMode(true);
        joinTeamButton.interactable = true;
    }
}
