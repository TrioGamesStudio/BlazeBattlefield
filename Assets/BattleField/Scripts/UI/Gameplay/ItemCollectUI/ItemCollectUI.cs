using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ItemCollectUI : BaseUIItem, IPoolCallback<ItemCollectUI>
{
    public Action<ItemCollectUI> OnCallback { get; set; }

    public void OnRelease()
    {
        OnCallback?.Invoke(this);
    }

    public void Initialize(IRunTimeItem itemInGame)
    {
        SetItemCount(itemInGame.Quantity());
        SetItemName($"{itemInGame.DisplayName()}");
        icon.sprite = itemInGame.GetIcon();
    }
}
