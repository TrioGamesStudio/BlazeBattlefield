using UnityEngine;

[CreateAssetMenu(fileName = "Item_",menuName = "Item_Data")]
public class ItemDataSO : ScriptableObject
{
    public GameObject modelPrefab;
    public ItemType ItemType;
    public string ItemName;
    public Sprite icon;
}