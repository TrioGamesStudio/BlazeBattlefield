using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IconMessageSO",menuName = "IconMessageSO")]
public class IconMessageSO : ScriptableObject
{
    private Dictionary<MessageLogType, Sprite> keyValuePairs;

    [SerializeField] private Sprite KillLog;
    [SerializeField] private Sprite JoinLog;
    [SerializeField] private Sprite LeaveLog;
    [SerializeField] private Sprite FallOffLog;
    public Sprite GetIcon(MessageLogType type)
    {
        if(keyValuePairs == null || keyValuePairs.Count == 0)
        {
            keyValuePairs = new()
            {
                { MessageLogType.KillLog, KillLog },
                { MessageLogType.JoinLog, JoinLog },
                { MessageLogType.LeaveLog, LeaveLog },
                { MessageLogType.FallOff, FallOffLog }
            };
        }
        if(keyValuePairs.TryGetValue(type,out var sprite))
        {
            return sprite;
        }
        return null;
    }
}