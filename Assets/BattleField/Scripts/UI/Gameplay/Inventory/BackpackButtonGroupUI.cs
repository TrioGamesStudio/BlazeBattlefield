using NaughtyAttributes.Test;
using System;
using UnityEngine;
using UnityEngine.UI;

public class BackpackButtonGroupUI : MonoBehaviour
{
    [SerializeField] private Button dropButton;
    [SerializeField] private Button dropAllButton;
    [SerializeField] private Button useButton;
    private event Action OnDropFullItem;
    private event Action OnShowDropButton;
    private event Action OnUseItem;
    private void Awake()
    {
        dropButton.onClick.AddListener(Drop);
        dropAllButton.onClick.AddListener(DropAll);
        useButton.onClick.AddListener(Use);
    }

    private void OnDestroy()
    {
        dropButton.onClick.RemoveListener(Drop);
        dropAllButton.onClick.RemoveListener(DropAll);
        useButton.onClick.RemoveListener(Use);
    }
    public void SetOnDropFull(Action dropAllItem)
    {
        OnDropFullItem = dropAllItem;
    }

    public void SetOndropItemAmount(Action showDropAmount)
    {
        OnShowDropButton = showDropAmount;
    }

    public void SetOnUseItem(Action useItem)
    {
        OnUseItem = useItem;
    }
    private void Drop()
    {
        OnShowDropButton?.Invoke();
    }

    public void DropAll()
    {
        OnDropFullItem?.Invoke();
    }
    
    private void Use()
    {
        OnUseItem?.Invoke();
    }

    public void RemoveAllRegister()
    {
        dropButton.onClick.RemoveAllListeners();
        dropAllButton.onClick.RemoveAllListeners();
        useButton.onClick.RemoveAllListeners();
    }

    

    public void SetupView(InventoryItem currentItem)
    {
        if(currentItem.ItemType == ItemType.Health)
        {
            useButton.gameObject.SetActive(true);
        }
        else
        {
            useButton.gameObject.SetActive(false);
        }
    }
}