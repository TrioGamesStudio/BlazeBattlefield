using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingPanel : MonoBehaviour
{
    public void SignOut()
    {
        LoginManager.Instance.SignOut();
        Matchmaking.Instance.LeaveRoom();
    }
}
