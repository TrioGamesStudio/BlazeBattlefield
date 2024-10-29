using NaughtyAttributes.Test;
using System;
using UnityEngine;
using UnityEngine.UI;

public class BackpackButtonGroupUI : MonoBehaviour
{
    [SerializeField] private Button dropButton;
    [SerializeField] private Button dropAllButton;
    [SerializeField] private Button useButton;
    [SerializeField] private Button equipButton;
    private event Action OnDropFullItem;
    private event Action OnShowDropButton;
    private void Awake()
    {
        dropButton.onClick.AddListener(Drop);
        dropAllButton.onClick.AddListener(DropAll);
    }

    private void OnDestroy()
    {
        dropButton.onClick.RemoveListener(Drop);
        dropAllButton.onClick.RemoveListener(DropAll);
    }

    private void Drop()
    {
        OnShowDropButton?.Invoke();
    }

    public void DropAll()
    {
        OnDropFullItem?.Invoke();
    }

    public void RemoveAllRegister()
    {
        dropButton.onClick.RemoveAllListeners();
        dropAllButton.onClick.RemoveAllListeners();
        useButton.onClick.RemoveAllListeners();
        equipButton.onClick.RemoveAllListeners();
    }

    public void SetOnDropFull(Action dropAllItem)
    {
        OnDropFullItem = dropAllItem;
    }

    public void SetOndropItemAmount(Action showDropAmount)
    {
        OnShowDropButton = showDropAmount;
    }
}