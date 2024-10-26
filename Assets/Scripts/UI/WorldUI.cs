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
    public TextMeshProUGUI topText;
    public TextMeshProUGUI topTextTeam;

    private void Start()
    {
        lobbyButton.onClick.AddListener(BackToLobby);
        lobbyButtonWin.onClick.AddListener(BackToLobby);
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

    public void ShowHideEliminateUI()
    {
        panelResultEliminate.SetActive(true);
    }

    public void SetText(string text)
    {
        informationText.text = text;
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
            yield return new WaitForSeconds(1f);             // Wait for 1 second
            informationText.text = countdownValue.ToString();  // Update the countdown text
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
        Debug.Log("BACK TO LOBBY NE");
        FindObjectOfType<Matchmaking>().BackToLobby();
    }

    public void BackToLobbyTeam()
    {
        Debug.Log("TEAM MEMBER BACK TO LOBBY NE");
        FindObjectOfType<MatchmakingTeam>().BackToLobby();
    }
}

