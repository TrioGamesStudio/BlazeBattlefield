using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;

public class AlivePlayerControl : MonoBehaviour
{
    public static Action OnUpdateAliveCountAction;

    private void Awake()
    {
        OnUpdateAliveCountAction += OnUpdateAliveCount;
    }
    private void OnDestroy()
    {
        OnUpdateAliveCountAction -= OnUpdateAliveCount;
    }
    [Button]
    private void OnUpdateAliveCount()
    {
        StartCoroutine(DelayUpdateUI());
        //AliveKillUI.UpdateAliveCount?.Invoke(GetAlivePlayer());
    }
    private IEnumerator DelayUpdateUI()
    {
        yield return new WaitForSeconds(.3f);
        AliveKillUI.UpdateAliveCount?.Invoke(GetAlivePlayer());
    }
    private int GetAlivePlayer()
    {
        PlayerRoomController[] players = FindObjectsByType<PlayerRoomController>(FindObjectsSortMode.None);
        int aliveCount = 0;
        Debug.Log("PLAYER COUNT:" + players.Length, gameObject);
        foreach(var item in players)
        {
            if (item.IsAlive)
            {
                aliveCount++;
            }
        }
        return aliveCount;
    }
}
