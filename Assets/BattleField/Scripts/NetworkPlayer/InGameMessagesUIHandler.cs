using System.Collections;
using UnityEngine;
using TMPro;

//todo gameObject = Player.Go -> Canvas_InGameMessages show info from stateAuthority 3 row
public class InGameMessagesUIHandler : MonoBehaviour
{
    public TextMeshProUGUI[] textMeshProUGUIs;
    Queue messageQueue = new Queue();

    public void OnGameMessageRecieved(string message) {
        Debug.Log($"InGameMessageUIHandler {message}");
        messageQueue.Enqueue(message);
        if(messageQueue.Count > 3) messageQueue.Dequeue();

        int queueIndex = 0;
        foreach (string messageQueue in messageQueue)
        {
            textMeshProUGUIs[queueIndex].text = messageQueue;
            queueIndex++;
        }
    }
}