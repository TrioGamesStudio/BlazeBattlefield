using System;
using UnityEngine;
using UnityEngine.UI;

public class BackpackButtonGroupUI : MonoBehaviour
{
    public Button dropButton;
    public Button dropAllButton;
    public Button useButton;
    public Button equipButton;
    private ItemLocalData currentItemData;
    public ItemLocalData GetCurrentItem()
    {
        return currentItemData;
    }
    public void ShowByIndex(int index)
    {
        transform.gameObject.SetActive(true);
        transform.SetSiblingIndex(index + 1);
    }
    public void SetCurrentItem(ItemLocalData customObject)
    {
        currentItemData = customObject;

    }
    public void Hide()
    {
        transform.gameObject.SetActive(false);
    }

    public void RemoveAllRegister()
    {
        dropButton.onClick.RemoveAllListeners();
        dropAllButton.onClick.RemoveAllListeners();
        useButton.onClick.RemoveAllListeners();
        equipButton.onClick.RemoveAllListeners();
    }

   
}
