using System.Collections.Generic;
#if UNITY_EDITOR
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "Hat Data Handler", menuName = "Skin Data Handler/Hat Variant")]
public class HatDataHandler : SkinDataHandler
{
    public List<GameObject> hatsList = new();

}