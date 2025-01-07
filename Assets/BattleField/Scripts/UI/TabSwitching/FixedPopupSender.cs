using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class FixedPopupSender : MonoBehaviour
{
    public Transform spawnTransform;
    public PopupMessage mess;
    public GameObject popupHolder;
    public int fontSize = 36;
    public float yAsixHeight = 50;

    private void Awake()
    {
        mess.gameObject.SetActive(false);
    }



    public void Popup(string message)
    {
        mess.DOKill();
        mess.gameObject.SetActive(true);

        Sequence mySequence = DOTween.Sequence();

        mess.transform.position = spawnTransform.position;
        mess.textMeshProUGUI.text = message;
        mess.gameObject.SetActive(true);
        mess.textMeshProUGUI.DOFade(0, 0);
        mess.transform.localScale = Vector3.zero;

        float newHeight = mess.transform.position.y + yAsixHeight;
        mySequence.Join(mess.textMeshProUGUI.DOFade(1, .25f));
        mySequence.Append(mess.transform.DOScale(Vector3.one, .25f));
        // mySequence.AppendInterval(.25f);
        mySequence.Append(mess.transform.DOMoveY(newHeight, .25f));
        mySequence.Append(mess.textMeshProUGUI.DOFade(0, .25f));
        mySequence.OnComplete(() =>
        {
            Debug.Log("On complete sequence, ");
            mess.gameObject.SetActive(false);
        });

        mySequence.PrependInterval(.5f);
        mySequence.SetTarget(mess);
    }

    public string testinString;
    [Button]
    private void Testing()
    {
        Popup(testinString);
    }

}