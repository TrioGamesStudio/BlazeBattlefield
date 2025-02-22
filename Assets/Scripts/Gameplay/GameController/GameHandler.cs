using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public static GameHandler instance;
    public Dictionary<string, List<PlayerRoomController>> teamsOriginal = new();
    public Dictionary<string, List<PlayerRoomController>> teams = new();
    private string localTeamID;
    [SerializeField] private Transform[] routePoints;

    private void Awake()
    {
        instance = this;
    }

    public void InitializeTeams()
    {
        teamsOriginal.Clear();
        teams.Clear();
        PlayerRoomController[] players = FindObjectsByType<PlayerRoomController>(FindObjectsSortMode.None);

        Debug.Log("===Players count: " + players.Length);

        foreach (var player in players)
        {
            if (teams.ContainsKey(player.TeamID.ToString()))
            {
                teams[player.TeamID.ToString()].Add(player);
            }
            else
            {
                List<PlayerRoomController> listPlayers = new();
                listPlayers.Add(player);
                teams[player.TeamID.ToString()] = listPlayers;
            }
            if (player.isLocalPlayer)
                localTeamID = player.TeamID.ToString();
        }

        foreach (var player in players)
        {
            if (teamsOriginal.ContainsKey(player.TeamID.ToString()))
            {
                teamsOriginal[player.TeamID.ToString()].Add(player);
            }
            else
            {
                List<PlayerRoomController> listPlayers = new();
                listPlayers.Add(player);
                teamsOriginal[player.TeamID.ToString()] = listPlayers;
            }
            if (player.TeamID == localTeamID)
            {
                NetworkPlayer networkPlayer = player.GetComponent<NetworkPlayer>();
                networkPlayer.SetNicknameUIColor(Color.blue); //Set teamate name plate UI color to blue
                if (!player.isLocalPlayer)
                    FindObjectOfType<TeammateInfo>().CreateTeammemberInfo(networkPlayer.nickName_Network.ToString(), 100, player.GetComponent<HPHandler>());
            }
            else
            {
                NetworkPlayer networkPlayer = player.GetComponent<NetworkPlayer>();
                if (networkPlayer != null)
                    networkPlayer.SetNicknameUIColor(Color.red); //Set enemy name plate UI color to red
            }
        }
        Debug.Log("===Team count: " + teams.Count);
        AlivePlayerControl.OnUpdateAliveCountAction?.Invoke();
        if (teams.Count == 1)
            CheckWin();
    }

    public void Eliminate(string teamID, PlayerRoomController player)
    {
        if (!teams.ContainsKey(teamID)) return;
        teams[teamID].Remove(player);
        if (teams[teamID].Count == 0)
        {
            teams.Remove(teamID);
        }
        if (teams.Count == 1) CheckWin();
        AlivePlayerControl.OnUpdateAliveCountAction?.Invoke();
    }

    public IEnumerator CheckLose(string teamID)
    {
        ;
        yield return new WaitForSeconds(2);
        if (!teams.ContainsKey(teamID)) //All teammate eliminated
        {
            //int ranking = teams.Count + 1;
            int ranking;
            if (Matchmaking.Instance.currentMode == Matchmaking.Mode.Solo)
                ranking = CheckRanking() + 1;
            else
                ranking = teams.Count + 1;
            Debug.Log("===Rank " + ranking);
            foreach (var playerRoomControl in teamsOriginal[teamID])
            {
                if (playerRoomControl != null)
                    playerRoomControl.RPC_ShowLose(ranking);
            }
        }
        else
        {
            if (!teamID.Contains("AI") && Matchmaking.Instance.currentMode != Matchmaking.Mode.Solo)
                FindObjectOfType<WorldUI>().ShowEliminateUI();
            //else if (!teamID.Contains("AI"))
            //{
            //    int ranking = teams.Count + 1;
            //    teamsOriginal[teamID].First().RPC_ShowLose(ranking);
            //}
        }
        CheckWin();
    }

    public void CheckWin()
    {
        if (teams.Count == 1) //Remain only one team
        {
            string teamID = teams.Keys.First();
            Debug.Log("===Victory team: " + teamID);
            if (teamID.Contains("AI")) return;

            foreach (var playerRoomControl in teamsOriginal[teamID])
            {
                if (playerRoomControl != null)
                    playerRoomControl.RPC_ShowWin();
            }
        }
    }

    public void AssignRoute()
    {
        BotAINetwork[] botAIs = FindObjectsOfType<BotAINetwork>();
        foreach (var botAI in botAIs)
        {
            botAI.SetRoutePoints(routePoints);
        }
    }

    private int CheckRanking()
    {
        int playerLive = 0;
        foreach (var item in FindObjectsOfType<PlayerRoomController>())
        {
            if (item.IsAlive)
            {
                playerLive++;
            }
        }
        return playerLive;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw route
        if (routePoints != null && routePoints.Length > 0)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < routePoints.Length; i++)
            {
                if (routePoints[i] != null)
                {
                    // Draw point
                    Gizmos.DrawSphere(routePoints[i].position, 0.5f);

                    // Draw line to next point
                    if (i + 1 < routePoints.Length && routePoints[i + 1] != null)
                    {
                        Gizmos.DrawLine(routePoints[i].position, routePoints[i + 1].position);
                    }
                    else if (i == routePoints.Length - 1 && routePoints[0] != null)
                    {
                        Gizmos.DrawLine(routePoints[i].position, routePoints[0].position);
                    }
                }
            }
        }

        // Draw detection and collection ranges
        Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(transform.position, dropBoxDetectionRadius);

        Gizmos.color = Color.green;
        //Gizmos.DrawWireSphere(transform.position, collectDistance);
    }

    [Button]
    private void CheckTeam()
    {
        int deathCount = 0;
        int aliveCount = 0;
        foreach (var item in teams)
        {
            foreach (var _item in item.Value)
            {
                if (_item.IsAlive)
                {
                    aliveCount++;
                }
                else
                {
                    deathCount++;
                }
            }

        }
        Debug.Log("Alive Count: " + aliveCount);
        Debug.Log("Death count: " + deathCount);
    }
}
