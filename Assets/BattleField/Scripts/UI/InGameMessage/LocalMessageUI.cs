using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI invokerNameText;
    [SerializeField] private TextMeshProUGUI listenerNameText;
    [SerializeField] private Image iconActionImg;

    public void PassMessageData(string _invokerName,string _listenerName)
    {
        invokerNameText.text = _invokerName;
        listenerNameText.text = _listenerName;
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}

