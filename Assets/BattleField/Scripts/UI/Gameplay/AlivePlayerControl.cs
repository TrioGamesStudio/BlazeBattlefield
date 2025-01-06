using NaughtyAttributes;
using System;
using UnityEngine;

public class AlivePlayerControl : MonoBehaviour
{
    public static Action<int> OnUpdateAliveCountAction;
    static int alivePlayer = 0;

    private void Awake()
    {
        OnUpdateAliveCountAction += OnUpdateAliveCount;
    }
    private void OnDestroy()
    {
        OnUpdateAliveCountAction -= OnUpdateAliveCount;
    }
    [Button]
    private void OnUpdateAliveCount(int alive)
    {
        alivePlayer = alive;
    
        AliveKillUI.UpdateAliveCount?.Invoke(alive);
    }

    public static void UpdateAliveCount(int amount)
    {
        alivePlayer -= amount;
        AliveKillUI.UpdateAliveCount?.Invoke(alivePlayer);
    }
}
