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
        //int alive = 0;
        //if (Matchmaking.Instance.currentMode == Matchmaking.Mode.Solo)
        //{
        //    foreach (var item in Matchmaking.Instance.players.Values)
        //    {
        //        if (item.IsAlive)
        //        {
        //            alive++;
        //        }
        //    }
        //}
        //else
        //{
        //    foreach (var item in MatchmakingTeam.Instance.players.Values)
        //    {
        //        if (item.IsAlive)
        //        {
        //            alive++;
        //        }
        //    }
        //}
        AliveKillUI.UpdateAliveCount?.Invoke(alive);
    }

    public static void UpdateAliveCount(int amount)
    {
        alivePlayer -= amount;
        AliveKillUI.UpdateAliveCount?.Invoke(alivePlayer);
    }
}
