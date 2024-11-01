using NaughtyAttributes;
using System;
using UnityEditor.VersionControl;
using UnityEditor;
using UnityEngine;

public class ItemConfig<SubType> : ScriptableObject where SubType : Enum
{
    public string displayName;
    public Sprite Icon;
    public SubType SubItemType;
    public int maxStack;
    public ItemType ItemType;

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

    public virtual void ShowDebug()
    {
        Debug.Log($"Item Name: {displayName}, MaxStack {maxStack} MainType: {ItemType.ToString()} SubType:{SubItemType.ToString()}");
    }

    public virtual int GetEnumIndex()
    {
        return (int)(object)SubItemType;
    }

}
