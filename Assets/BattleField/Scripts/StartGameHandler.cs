using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameHandler : MonoBehaviour
{
    public static Action OnStartGameAction;
    public GameHandler gameHandler;
    public WaitingArea waitingArea;
    public UIController UIController;
    private void Awake()
    {
        OnStartGameAction += PassAllPlayer;
        
    }
    private void OnDestroy()
    {
        OnStartGameAction -= PassAllPlayer;
    }

    public void PassAllPlayer()
    {
        StartCoroutine(PlayerCoroutine());
    }
    private IEnumerator PlayerCoroutine()
    {
        AlivePlayerControl.OnUpdateAliveCountAction?.Invoke();
        UIController = UIController.Instance;
        //foreach (var playerRoomController in MatchmakingTeam.Instance.players.Values)
        //{
        //    Debug.Log("Stop input", gameObject);
        //    Debug.Log("Play close eye effect", gameObject);
        //    Debug.Log("Random", gameObject);
        //    Debug.Log("open eye", gameObject);
        //}
        gameHandler.InitializeTeams();
        if (UIController)
        {
            UIController.StartCountdown();
            Debug.Log("Start coundown");
        }
        else
        {
            Debug.Log("dont have UI to countdown");
        }
        yield return new WaitForSeconds(3.1f);
        NetworkPlayer.Local.GetComponent<CharacterMovementHandler>().RespawnOnStartingBattle();

    }

    public void StopAllPlayer()
    {

    }
}
