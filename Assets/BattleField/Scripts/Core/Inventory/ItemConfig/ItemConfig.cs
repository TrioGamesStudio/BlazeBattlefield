using NaughtyAttributes;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
public enum ItemRarity
{
    Common,
    Rare,
    Epic
}
public class ItemConfig<SubType> : ScriptableObject where SubType : Enum
{
    public string displayName;
    public Sprite Icon;
    public SubType SubItemType;
    public int maxStack;
    public ItemType ItemType;

    public ItemRarity ItemRarity;
    [MinValue(0),MaxValue(100)] public byte ItemWeight;

#if UNITY_EDITOR
    [Button]
    private void RenameByDisplayName()
    {
        if (displayName == String.Empty)
        {
            name = "Item_DisplayNameIsEmpty";
        }
        else
        {
            string assetPath = AssetDatabase.GetAssetPath(GetInstanceID());
            AssetDatabase.RenameAsset(assetPath, displayName);
            AssetDatabase.SaveAssets();
        }
    }
#endif
    public virtual void ShowDebug()
    {
        Debug.Log($"Item Name: {displayName}, MaxStack {maxStack} MainType: {ItemType.ToString()} SubType:{SubItemType.ToString()}");
    }

    public virtual int GetEnumIndex()
    {
        return (int)(object)SubItemType;
    }

}
