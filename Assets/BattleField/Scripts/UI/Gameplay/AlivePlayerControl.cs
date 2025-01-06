using NaughtyAttributes;
using System;
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
    
        AliveKillUI.UpdateAliveCount?.Invoke(GetAlivePlayer());
    }

    private int GetAlivePlayer()
    {
        PlayerRoomController[] players = FindObjectsByType<PlayerRoomController>(FindObjectsSortMode.None);
        int aliveCount = 0;
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
