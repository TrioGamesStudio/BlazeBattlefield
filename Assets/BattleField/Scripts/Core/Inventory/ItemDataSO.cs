using Fusion;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_",menuName = "Item_Data")]
public class ItemDataSO : ScriptableObject
{
    public ItemInGame modelPrefab;
    public ItemType ItemType;
    public string ItemName;
    public Sprite icon;
    public int WeaponSlot;
    public bool canStackable;
    public int maxCountPerStack = 1;
}