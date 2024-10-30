using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public Dictionary<string, List<PlayerRoomController>> teamsOriginal = new();
    public Dictionary<string, List<PlayerRoomController>> teams = new();

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
    }

    public void Eliminate(string teamID, PlayerRoomController player)
    {
        Debug.Log("===Eliminate player" + player.TeamID + " in local");
        teams[teamID].Remove(player);
        if (teams[teamID].Count == 0)
        {
            Debug.Log("===BEFORE remove");
            foreach (var key in teams.Keys)
            {
                Debug.Log("===Key: " + key);
                foreach (var playerRoom in teams[key])
                {
                    Debug.Log("====Player team id: " + playerRoom.TeamID);
                }
            }
            teams.Remove(teamID);
            Debug.Log("===AFTER remove");
            foreach (var key in teams.Keys)
            {
                Debug.Log("===Key: " + key);
                foreach (var playerRoom in teams[key])
                {
                    Debug.Log("====Player team id: " + playerRoom.TeamID);
                }
            }
            Debug.Log("===Remain team after remove " + teams.Count);
        }
    }

    public IEnumerator CheckLose(string teamID)
    {
        //await Task.Delay(500);
        yield return new WaitForSeconds(.5f);
        if (!teams.ContainsKey(teamID)) //All teammate eliminated
        //if (teams[teamID].Count == 0) 
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
            Debug.Log("===Team " + teamID + " remain " + teams[teamID].Count + " player");
            Debug.Log("===Remain teammate alive -> Watch or leave");
            FindObjectOfType<WorldUI>().ShowEliminateUI();
        }
        CheckWin();
    }

    public void CheckWin()
    {
        if (teams.Count == 1) //Remain only one team
        {
            string teamID = teams.Keys.First();
            Debug.Log("===Victory team: " + teamID);

            foreach(var playerRoomControl in teamsOriginal[teamID])
            {
                if (playerRoomControl != null)
                    playerRoomControl.RPC_ShowWin();
            }
        }
    }
}
