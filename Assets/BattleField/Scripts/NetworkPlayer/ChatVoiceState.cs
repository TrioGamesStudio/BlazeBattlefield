
using UnityEngine;
using UnityEngine.UI;

public class ChatVoiceState : MonoBehaviour
{
    [SerializeField] GameObject lockChatVoiceImage;
    [SerializeField] GameObject unLockChatVoiceImage;

    private void Awake() {
        lockChatVoiceImage.SetActive(true);
        unLockChatVoiceImage.SetActive(false);
    }

    public void AvtiveChatVoiceButton() {
        lockChatVoiceImage.SetActive(false);
        unLockChatVoiceImage.SetActive(true);
    }

    public void DeAvtiveChatVoiceButton() {
        lockChatVoiceImage.SetActive(true);
        unLockChatVoiceImage.SetActive(false);
    }
}
