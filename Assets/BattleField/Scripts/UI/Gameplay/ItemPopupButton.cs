using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ItemPopupButton : MonoBehaviour
{
    [SerializeField] private Button PopUpButton;
    [SerializeField] private Button PopInButton;
    [SerializeField] private GameObject PopView;
    [SerializeField] private Button SelectButtonPrefab;
    private List<Button> selectBtnList = new();
    public Action UseItem;
    public int itemCount = 3;

    private void Awake()
    {
        PopUpButton.onClick.AddListener(PopUp);
        PopInButton.onClick.AddListener(PopIn);

        PopView.SetActive(false);

        SelectButtonPrefab.gameObject.SetActive(false);
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
            // assign callback for use item
            selectBtn.onClick.AddListener(() =>
            {
                Debug.Log("Add button callback", gameObject);
            });
            
            
            // update button information like sprite and count
            Sprite sprite = null;
            int count = Random.Range(0, 4);
            selectBtn.interactable = count != 0;
            selectBtn.image.sprite = sprite != null ? sprite : selectBtn.image.sprite;
            selectBtn.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"x{count}";
            selectBtn.onClick.AddListener(() =>
            {
                // add event callback
                // change this item to current item to use
            });
            selectBtnList.Add(selectBtn);
        }
    }

    private void OnDestroy()
    {
        PopUpButton.onClick.RemoveListener(PopUp);
        PopInButton.onClick.RemoveListener(PopIn);
    }


    private void PopUp()
    {
        Debug.Log("PopUp");
        ReloadView(itemCount);
        PopView.SetActive(true);
    }

    private void PopIn()
    {
        Debug.Log("PopIn");
        PopView.SetActive(false);
    }
}