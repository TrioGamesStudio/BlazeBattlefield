using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameHandler : MonoBehaviour
{
    public static StartGameHandler instance;
    public static Action OnStartGameAction;
    public GameHandler gameHandler;
    public WaitingArea waitingArea;
    public UIController UIController;
    public PlaySoundReady PlaySoundReady;
    private void Awake()
    {
        instance = this;
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
        if (UIController)
        {
            UIController.StartCountdown();
            Debug.Log("Start coundown");
        }
        else
        {
            Debug.Log("dont have UI to countdown");
        }
        yield return new WaitForSeconds(6.1f);
        PlaySoundReady.PlayReadySound();
        yield return new WaitForSeconds(2);
        gameHandler.InitializeTeams();
        waitingArea.ReleasePlayer();
        NetworkPlayer.Local.GetComponent<CharacterMovementHandler>().RespawnOnStartingBattle();
        Debug.Log("Start spawn");
    }

    public void StopAllPlayer()
    {

    }
}
