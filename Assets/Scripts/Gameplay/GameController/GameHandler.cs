using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public Dictionary<string, List<PlayerRoomController>> teamsOriginal = new();
    public Dictionary<string, List<PlayerRoomController>> teams = new();
    private string localTeamID;
    [SerializeField] private Transform[] routePoints;

    public void InitializeTeams()
    {
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
        Debug.Log("===AFTER initialize");
        foreach (var key in teams.Keys)
        {
            Debug.Log("===Key: " + key);
            foreach (var playerRoom in teams[key])
            {
                Debug.Log("====Player team id: " + playerRoom.TeamID);
            }
        }
        Debug.Log("===Team count: " + teams.Count);
        AlivePlayerControl.OnUpdateAliveCountAction?.Invoke(players.Count());
        if (teams.Count == 1)
            CheckWin();
    }

    public void Eliminate(string teamID, PlayerRoomController player)
    {
        //Debug.Log("===Eliminate player" + player.TeamID + " in local");
        if (!teams.ContainsKey(teamID)) return;
        teams[teamID].Remove(player);
        if (teams[teamID].Count == 0)
        {
            Debug.Log("===BEFORE remove");
            foreach (var key in teams.Keys)
            {
                Debug.Log("===Key: " + key);
                foreach (var playerRoom in teams[key])
                {
                    //Debug.Log("====Player team id: " + playerRoom.TeamID);
                }
            }
            teams.Remove(teamID);
            Debug.Log("===AFTER remove");
            foreach (var key in teams.Keys)
            {
                Debug.Log("===Key: " + key);
                foreach (var playerRoom in teams[key])
                {
                    //Debug.Log("====Player team id: " + playerRoom.TeamID);
                }
            }
            Debug.Log("===Remain team after remove " + teams.Count);
        }
        if (teams.Count == 1) CheckWin();
        AlivePlayerControl.UpdateAliveCount(1);
    }

    public IEnumerator CheckLose(string teamID)
    {
        //await Task.Delay(500);
        yield return new WaitForSeconds(2);
        if (!teams.ContainsKey(teamID)) //All teammate eliminated
        {
            int ranking = teams.Count + 1;
            //Debug.Log("===No teammate remain -> Defeat " + "Top " + ranking);
            foreach (var playerRoomControl in teamsOriginal[teamID])
            {
                if (playerRoomControl != null)
                    playerRoomControl.RPC_ShowLose(ranking);
            }
        }
        else
        {
            //Debug.Log("===Team " + teamID + " remain " + teams[teamID].Count + " player");
            //Debug.Log("===Remain teammate alive -> Watch or leave");
            if (!teamID.Contains("AI") && Matchmaking.Instance.currentMode != Matchmaking.Mode.Solo) 
                FindObjectOfType<WorldUI>().ShowEliminateUI();
            else if (!teamID.Contains("AI"))
            {
                int ranking = teams.Count + 1;
                //Debug.Log("===No teammate remain -> Defeat " + "Top " + ranking);
                //foreach (var playerRoomControl in teamsOriginal[teamID])
                //{
                //    if (playerRoomControl != null)
                //        playerRoomControl.RPC_ShowLose(ranking);
                //}
                teamsOriginal[teamID].First().RPC_ShowLose(ranking);
            }
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

            foreach(var playerRoomControl in teamsOriginal[teamID])
            {
                if (playerRoomControl != null)
                    playerRoomControl.RPC_ShowWin();
            }
        }
    }

    public void AssignRoute()
    {
        //BotAI[] botAIs = FindObjectsOfType<BotAI>();
        //foreach(var botAI in botAIs)
        //{
        //    botAI.SetRoutePoints(routePoints);
        //}
        BotAINetwork[] botAIs = FindObjectsOfType<BotAINetwork>();
        foreach (var botAI in botAIs)
        {
            botAI.SetRoutePoints(routePoints);
        }
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

}
