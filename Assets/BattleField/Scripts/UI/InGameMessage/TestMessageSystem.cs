using Fusion;
using NaughtyAttributes;
using UnityEngine;

public class TestMessageSystem : NetworkBehaviour
{
    private void Awake()
    {
    }
    public string playerName1 = "Player 1";
    public string playerName2 = "Player 2";
    [EditorButton]
    private void TestFall()
    {
        GetComponent<PlayerMessageManager>().Fall(playerName1);
    }
    [EditorButton]
    private void TestKill()
    {
        GetComponent<PlayerMessageManager>().SendKillLogRPC(playerName1,playerName2);
    }
    [EditorButton]
    private void TestExit()
    {
        GetComponent<PlayerMessageManager>().ExitLogRPC(playerName1);
    }
    [EditorButton]
    private void TestEnter()
    {
        GetComponent<PlayerMessageManager>().EnterLogRPC(playerName1);
    }
}