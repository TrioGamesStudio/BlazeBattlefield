using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MessageDisplayUI : MonoBehaviour
{
    [SerializeField] private List<LocalMessageUI> uiElements = new();

    [SerializeField] private GameObject view;
    [SerializeField] private LocalMessageUI messagePrefab;
    
    [SerializeField] private int maxMessages = 3;
    [SerializeField] private float offsetY = 25;
    [Serializable]
    public struct LocalMessageControl
    {
        public LocalMessageUI localMessageUI;
        public float timer;
        
    }

    private void Awake()
    {
        messagePrefab.gameObject.SetActive(false);
    }
    [Header("Testing")]
    public string customMessage;
    private int index = 0;
    [Button]
    private void Test()
    {
        CreateMessage(index.ToString());
        index++;
    }

    public void CreateMessage(string message)
    {
        var messageUI = CreateInstance();
        messageUI.gameObject.SetActive(true);
        messageUI.name = "Local Message" + uiElements.Count;
        messageUI.DestroyCallback += () =>
        {
            uiElements.Remove(messageUI);
            Destroy(messageUI.gameObject);
        };

        RemoveOldestMessage();

        SortingMessagesPosition();

    }

  

    private LocalMessageUI CreateInstance()
    {
        var ui = Instantiate(messagePrefab, view.transform);
        uiElements.Add(ui);
        return ui;
    }

    private void SortingMessagesPosition()
    {
        int indexCounting = 0;
        for (int i = uiElements.Count - 1; i >= 0; i--)
        {
            RectTransform rectTransform = uiElements[i].GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, indexCounting * offsetY);
            indexCounting++;
        }
    }

    private void RemoveOldestMessage()
    {
        if(uiElements.Count > maxMessages)
        {
            var oldestUI = uiElements[0];
            uiElements.Remove(oldestUI);
            Destroy(oldestUI.gameObject);
        }
    }
}