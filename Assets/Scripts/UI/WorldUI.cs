using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldUI : MonoBehaviour
{
    public GameObject panelResultLose;
    public GameObject panelResultWin;
    public GameObject panelResultWinTeam;
    public GameObject panelResultEliminate;
    public GameObject panelResultLoseTeam;
    public Button lobbyButton;
    public Button lobbyButtonWin;
    public TextMeshProUGUI informationText;
    public TextMeshProUGUI waitingTimeText;
    public TextMeshProUGUI topText;
    public TextMeshProUGUI topTextTeam;
    public StatsDisplay statsDisplay;

    // buttons
    [SerializeField] Button returnLobby;
    [SerializeField] GameObject returnLobbyPanel;
    bool isCursorShowed = false;

    private void Start()
    {
        lobbyButton.onClick.AddListener(BackToLobby);
        lobbyButtonWin.onClick.AddListener(BackToLobby);

        returnLobby.onClick.AddListener(BackToLobby);
        InputPlayerMovement.ExitAction += ToggleCursor;
    }

    private void OnDestroy()
    {
        InputPlayerMovement.ExitAction -= ToggleCursor;

    }   

    public void ShowHideUI(int alivePlayer)
    {
        topText.text = "TOP " + alivePlayer;
        panelResultLose.SetActive(!panelResultLose.activeSelf);
    }

    public void ShowHideUIDefeatTeam(int alivePlayer)
    {
        topTextTeam.text = "TOP " + alivePlayer;
        panelResultLoseTeam.SetActive(!panelResultLose.activeSelf);
    }

    public void ShowHideWinUI()
    {
        panelResultWin.SetActive(!panelResultLose.activeSelf);
    }

    public void ShowHideWinUITeam()
    {
        panelResultWinTeam.SetActive(!panelResultLose.activeSelf);
    }

    public void ShowEliminateUI()
    {
        panelResultEliminate.SetActive(true);
    }

    public void HideEliminateUI()
    {
        panelResultEliminate.SetActive(false);
    }

    public void SetText(string text)
    {
        informationText.gameObject.SetActive(true);
        informationText.text = "";
        informationText.text = text;
    }

    public void SetWaitingTime(string time)
    {
        waitingTimeText.gameObject.SetActive(true);
        waitingTimeText.text = $"Estimated time remaining: {time}";
    }

    public void TurnOffWaitingTime()
    {
        waitingTimeText.gameObject.SetActive(false);
    }

    public void StartCountdown()
    {
        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        int countdownValue = 3;  // Start from 3
        yield return new WaitForSeconds(1f);
        while (countdownValue > 0)
        {
            informationText.text = countdownValue.ToString();  // Update the countdown text
            yield return new WaitForSeconds(1f);             // Wait for 1 second
            //countdownText.gameObject.SetActive(true);        // Show the countdown UI        
            countdownValue--;
        }

        // Countdown finished
        informationText.text = "GO!";
        yield return new WaitForSeconds(1f);  // Keep "GO!" for 1 second

        // Hide the countdown UI
        informationText.gameObject.SetActive(false);

        // Load the battle scene
        //TransitionToBattleScene(runner);
    }

    private void BackToLobby()
    {
        FindObjectOfType<Matchmaking>().BackToLobbyAll();
    }

    public void BackToLobbyTeam()
    {
        FindObjectOfType<MatchmakingTeam>().BackToLobby();
    }

    // on off cursor
    void ToggleCursor() {
        isCursorShowed = !isCursorShowed;
        if(isCursorShowed) ShowCursor();
        else HideCursor();
    }
        
    void ShowCursor() {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void HideCursor() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ShowPlayerStats()
    {
        statsDisplay.gameObject.SetActive(true);
        statsDisplay.DisplayStats(PlayerStats.Instance);
    }
}

