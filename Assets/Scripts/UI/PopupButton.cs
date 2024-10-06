using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PopupButton : MonoBehaviour
{
    [SerializeField] private Button UseCurrentItemButton;
    [SerializeField] private Button PopUpButton;
    [SerializeField] private Button PopInButton;
    [SerializeField] private GameObject PopView;
    [SerializeField] private Button SelectButtonPrefab;
    private List<Button> selectBtnList = new();
    public Action UseItem;

    private void Awake()
    {
        UseCurrentItemButton.onClick.AddListener(UseCurrentItem);
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
            selectBtn.onClick.AddListener(() => { Debug.Log("Add button callback", gameObject); });
            // update button information like sprite and count
            Sprite sprite = null;
            int count = Random.Range(0, 4);
            SetupButtonVisual(selectBtn, sprite, count);
            selectBtnList.Add(selectBtn);
        }
    }

    private void SetupButtonVisual(Button button, Sprite sprite, int count)
    {
        button.interactable = count != 0;
        button.image.sprite = sprite != null ? sprite : button.image.sprite;
        button.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"x{count}";
    }

    private void OnDestroy()
    {
        UseCurrentItemButton.onClick.RemoveListener(UseCurrentItem);
        PopUpButton.onClick.RemoveListener(PopUp);
        PopInButton.onClick.RemoveListener(PopIn);
    }

    private void UseCurrentItem()
    {
        Debug.Log("Use current item");
    }

    public int itemCount = 3;

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