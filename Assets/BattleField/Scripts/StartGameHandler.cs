using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class StartGameHandler : MonoBehaviour
{
    public static StartGameHandler instance;
    public GameHandler gameHandler;
    public WaitingArea waitingArea;
    public UIController UIController;
    public PlaySoundReady PlaySoundReady;
    private void Awake()
    {
        instance = this;

    }

    public void PassAllPlayer()
    {
        Debug.Log("PassAllPlayer", gameHandler);
        NetworkPlayer.Local.StartCoroutine(PlayerCoroutine());
    }

    private IEnumerator PlayerCoroutine()
    {
        UIController = UIController.Instance;

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
        CameraEffectControl.instance.EyeBlinkEffect.CloseEye();

        yield return new WaitForSeconds(2);
        
        NetworkPlayer.Local.GetComponent<CharacterMovementHandler>().RespawnOnStartingBattle();
     
        gameHandler.InitializeTeams();
        gameHandler.AssignRoute();
        waitingArea.ReleasePlayer();
      
        yield return new WaitForSeconds(1);
        Debug.Log("Start spawn");
        CameraEffectControl.instance.EyeBlinkEffect.OpenEyeImmediately();
    }


}
