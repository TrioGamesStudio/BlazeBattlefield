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
    public event Action OnDropFullItem;
    public event  Action OnDropItem;
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
        OnDropItem?.Invoke();
        BackpackUI.instance.HideButton();
        BackpackUI.instance.HideDropAmount();
    }

    public void DropAll()
    {
        OnDropFullItem?.Invoke();
        BackpackUI.instance.HideButton();
        BackpackUI.instance.HideDropAmount();
    }

    public void ShowByIndex(int index)
    {
        transform.gameObject.SetActive(true);
        transform.SetSiblingIndex(index + 1);
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