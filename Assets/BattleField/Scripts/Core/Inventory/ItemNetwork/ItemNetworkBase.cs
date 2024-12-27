using Fusion;
using NaughtyAttributes;
using System;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
[Serializable]
public class CustomData
{
    public CustomData()
    {

    }
    public CustomData(string _key, float _value)
    {
        key = _key;
        value = _value;
    }
    public string key;
    public float value;
}

public interface ItemDataEnum
{
    Enum GetSubItemType();
    ItemType GetItemType();

    void SetQuantity(int newAmount);
    void SetCustomData(string key, float value);

    ItemRarity GetItemRarity();
    byte GetItemWeight();
}

public abstract class ItemNetworkBase<_EnumType, _Config> : NetworkBehaviour, ItemDataEnum,
    IRunTimeItem where _EnumType : Enum where _Config : ItemConfig<_EnumType>
{
    [Networked] public int quantity { get; set; }
    public Action<IRunTimeItem> OnRemoveItemUI { get; set; }
    public bool isDisplayedUI { get; set; }
    public CustomData[] customDatas;
    public _Config config;
    private BoundItem boundItem;

    public ItemRarity ItemRarity;
    [MinValue(0), MaxValue(100)] public byte ItemWeight;

    [SerializeField] private AudioClip collectSound;
    private void Awake()
    {
        boundItem = GetComponent<BoundItem>();
        collectSound = SoundManager.SoundAsset.GetSound("pickup_0");
    }
    public override void Spawned()
    {
        base.Spawned();
        if (HasStateAuthority)
        {
            //Invoke(nameof(BoundItemSetup), .3f);
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        if (isDisplayedUI)
        {
            OnRemoveItemUI?.Invoke(this);
        }
    }


    public void Collect()
    {

        //inventoryItem.maxStack = config.maxStack;
        //inventoryItem.ItemType = config.ItemType;
        //inventoryItem.displayName = config.displayName;
        //inventoryItem.Icon = config.Icon;
        //inventoryItem.amount = quantity;
        //inventoryItem._SubItemEnum = config.SubItemType;
        AddToStorage();

        DestroyItem();
    }

    public virtual void CollectAI(ActiveWeaponAI activeWeaponAI)
    {
        DestroyItem();
    }
  
    protected virtual void AddToStorage()
    {
        InventoryItem inventoryItem = new();
        inventoryItem.Create(config, quantity);
        StorageManager.instance.Add(config.ItemType, config.SubItemType, inventoryItem);
        NetworkPlayer.Local.GetComponent<AudioSource>().CustomPlaySound(collectSound);
    }
    
    public void DestroyItem()
    {
        DestroyRPC();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void DestroyRPC()
    {
        if (HasStateAuthority)
        {
            Debug.Log("Destroy item rpc: "+gameObject.name);
            Runner.Despawn(Object);
        }
    }

    [EditorButton]
    public void DestroyTest()
    {
        Object.RequestStateAuthority();
        DestroyRPC();
    }

    public Enum GetSubItemType()
    {
        return config.SubItemType;
    }

    public ItemType GetItemType()
    {
        return config.ItemType;
    }

    public string DisplayName()
    {
        return config.displayName;
    }

    string IRunTimeItem.UniqueID()
    {
        return Object.Id.ToString();
    }

    public int Quantity()
    {
        return quantity;
    }

    public Sprite GetIcon()
    {
        return config.Icon;
    }
    public void SetQuantity(int newAmount)
    {
        SetQuantityRPC(newAmount);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void SetQuantityRPC(int newAmount)
    {
        if (Object.HasStateAuthority)
        {
            quantity = newAmount;
        }
    }



    public void SetCustomData(string key, float value)
    {
        SetCustomDataRPC(key, value);
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void SetCustomDataRPC(string key, float value)
    {
        if (Object.HasStateAuthority)
        {
            customDatas[0].key = key;
            customDatas[0].value = value;
        }
    }

    public ItemRarity GetItemRarity()
    {
        return ItemRarity;
    }

    public byte GetItemWeight()
    {
        return ItemWeight;
    }

    public void DisableOutline()
    {
        rpc_disableOutline();
    }
    [Rpc(RpcSources.StateAuthority,RpcTargets.All)]
    private void rpc_disableOutline()
    {
        GetComponent<Outline>().enabled = false;
    }

    public void CollectAI()
    {
        throw new NotImplementedException();
    }
}
