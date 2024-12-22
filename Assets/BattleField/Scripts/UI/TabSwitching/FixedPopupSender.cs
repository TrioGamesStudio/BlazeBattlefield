using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class FixedPopupSender : MonoBehaviour
{
    public string popupMessage;

    public Transform spawnTransform;
    public PopupMessage mess;
    public GameObject popupHolder;
    public float yAsixHeight = 50;

    private void Awake()
    {
        mess.gameObject.SetActive(false);
    }
    [Button]
    public void SendMessage()
    {
        mess.DOKill();

        Sequence mySequence = DOTween.Sequence();
     
        mess.transform.position = spawnTransform.position;
        mess.textMeshProUGUI.text = popupMessage;
        mess.gameObject.SetActive(true);
        mess.textMeshProUGUI.DOFade(1, 0);
        
        float newHeight = mess.transform.position.y + yAsixHeight;
        
        mySequence.Append(mess.transform.DOMoveY(newHeight, .25f));
        mySequence.Append(mess.textMeshProUGUI.DOFade(0, .25f));
        mySequence.OnComplete(() =>
        {
            Debug.Log("On complete sequence, ");
        });

        mySequence.PrependInterval(.5f);
        mySequence.SetTarget(mess);
    }

}