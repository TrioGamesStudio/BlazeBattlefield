using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ItemPopupButton : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Button SelectButtonPrefab;
    [Header("Buttons")]
    [SerializeField] private Button PopUpButton;
    [SerializeField] private Button PopInButton;
    [Header("Views")]
    [SerializeField] private GameObject PopView;
    [Header("Auto Close Settings")]
    [SerializeField] private float timer;
    [SerializeField] private float popInTimeAfterPopUp = .25f;
    
    
    private List<Button> selectBtnList = new();
    public Action UseItem;
    private int itemCount = 3;
    private bool isShowView = false;
    
    private void Awake()
    {
        PopUpButton.onClick.AddListener(PopUp);
        PopInButton.onClick.AddListener(PopIn);

        PopView.SetActive(false);
        PopInButton.gameObject.SetActive(false);
        SelectButtonPrefab.gameObject.SetActive(false);
        isShowView = false;
    }

    private void OnDestroy()
    {
        PopUpButton.onClick.RemoveListener(PopUp);
        PopInButton.onClick.RemoveListener(PopIn);
    }
    private void ReloadView(int selectCount)
    {
        foreach (var btn in selectBtnList)
        {
            Destroy(btn.gameObject);
        }
        selectBtnList.Clear();
        
        for (int i = 0; i < selectCount; i++)
        {
            var selectBtn = Instantiate(SelectButtonPrefab, PopView.transform);
            selectBtn.gameObject.SetActive(true);
            selectBtn.onClick.AddListener(() =>
            {
                Debug.Log("Add button callback", gameObject);
            });
            
            
            // update button information like sprite and count
            Sprite sprite = null;
            int count = Random.Range(0, 4);
            
            selectBtn.interactable = count != 0;
            selectBtn.image.sprite = sprite != null ? sprite : selectBtn.image.sprite;
            selectBtn.transform.GetComponentInChildren<TextMeshProUGUI>().text = $"x{count}";
            selectBtn.onClick.AddListener(() =>
            {
                // add event callback
                // change this item to current item to use
            });
            selectBtnList.Add(selectBtn);
        }
    }



    private void PopUp()
    {
        Debug.Log("PopUp");
        
        ReloadView(itemCount);
        PopView.SetActive(true);
        ShowTwoPopButton(true, false);

        timer = popInTimeAfterPopUp;
        isShowView = true;
    }

    private void PopIn()
    {
        Debug.Log("PopIn");
        ShowTwoPopButton(false, true);
        PopView.SetActive(false);
        isShowView = false;
    }

    private void ShowTwoPopButton(bool showPopIn, bool showPopUp)
    {
        PopInButton.gameObject.SetActive(showPopIn);
        PopUpButton.gameObject.SetActive(showPopUp);
    }
    private void Update()
    {
        if (isShowView == false) return;
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            PopIn();
            timer = popInTimeAfterPopUp;
        }
    }
}