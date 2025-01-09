using System.Collections.Generic;
#if UNITY_EDITOR
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "Hat Data Handler", menuName = "Skin Data Handler/Hat Variant")]
public class HatDataHandler : SkinDataHandler
{
    [SerializeField] private List<GameObject> hatsList = new();
    public GameObject GetHatPrefabByIndex(int newIndex)
    {
        return hatsList[newIndex];
    }
}
