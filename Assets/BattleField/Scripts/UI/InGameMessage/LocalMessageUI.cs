using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Fusion.Editor.FusionHubWindow;

public class LocalMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI invokerNameText;
    [SerializeField] private TextMeshProUGUI listenerNameText;
    [SerializeField] private Image iconActionImg;

    [SerializeField] private TextMeshProUGUI fullText;
    [SerializeField] private Image fullTextIcon;
    [SerializeField] private float timer;
    [SerializeField] private float despawnTime = 3;
    [SerializeField] private bool isDespawn = false;

    public Action DestroyCallback;
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > despawnTime && isDespawn == false)
        {
            isDespawn = true;
            DestroyCallback?.Invoke();
        }
    }

    public void PassMessageData(string _invokerName, string _listenerName, Sprite sprite)
    {
        invokerNameText.text = _invokerName;
        listenerNameText.text = _listenerName;
        fullText.text = "";
        HandleImage(iconActionImg, sprite);
        HandleImage(fullTextIcon, null);

    }

    public void FullMessage(string message,Sprite icon)
    {
        fullText.text = message;
        HandleImage(fullTextIcon, icon);
        ResetCustomLog();
    }

    private void HandleImage(Image image, Sprite sprite)
    {
        if (sprite == null)
        {
            image.sprite = null;
            image.gameObject.SetActive(false);
        }
        else
        {
            image.sprite = sprite;
            image.gameObject.SetActive(true);
        }
    }

    private void ResetCustomLog()
    {
        invokerNameText.text = "";
        listenerNameText.text = "";
        HandleImage(iconActionImg, null);

    }
}

