using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinSelectionUI : MonoBehaviour
{
    public GameObject skinSelectUIPrefab;
    public GameObject container;
    public List<Sprite> skinSpriteIcons = new();
    [Button]
    public void Init()
    {
        foreach(var sprite in skinSpriteIcons)
        {
            var icon = Instantiate(skinSelectUIPrefab, container.transform);
            icon.transform.GetChild(1).GetComponent<Image>().sprite = sprite;
        }
    }
}
