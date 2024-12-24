using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class TabSwitchingUI : MonoBehaviour
{
    public byte Priority = 0;
    public byte Order = 0;
    public byte Index;
    public Sprite TabIcon;
    [Tooltip("You can use these button to hide current UI cover diffents UI exits")]
    [SerializeField] private List<GameObject> viewsObject = new();
    [Button("Show (Can use to show object quickly of this UI)")]
    public void Show()
    {
        foreach (var view in viewsObject)
        {
            view.SetActive(true);
        }
    }

    [Button("Hide (Can use to hide object quickly of this UI)")]
    public void Hide()
    {
        foreach (var view in viewsObject)
        {
            view.SetActive(false);
        }
    }
}
