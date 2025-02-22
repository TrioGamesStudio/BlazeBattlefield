﻿using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TabSwitchingManager : MonoBehaviour
{
    [SerializeField] private TabSwitchingUI[] tabSwitchingUIs;
    [SerializeField] private GameObject tabButtonContainer;
    [SerializeField] private TabSwtichButton tabButtonSwapPrefab;
    [SerializeField] private List<TabSwtichButton> buttonLists = new();

    public byte defaultTabIndex = 0;

    private void Awake()
    {
        tabSwitchingUIs = GetComponentsInChildren<TabSwitchingUI>();
        CreateTabButtonSwtiching();
    }

    private void CreateTabButtonSwtiching()
    {
        byte index = 0;
        foreach (var item in tabSwitchingUIs)
        {
            item.Index = index;

            var tabBtn = Instantiate(tabButtonSwapPrefab, tabButtonContainer.transform);

            tabBtn.SetIcon(item.TabIcon);
            tabBtn.OnShowTabUI = ShowTabUIByIndex;
            tabBtn.tabIndex = item.Index;

            index++;

            buttonLists.Add(tabBtn);
            // setup button
        }
    }

    public void ShowDefaultTabIndex()
    {
        ShowTabUIByIndex(defaultTabIndex);
    }

    public void ShowTabUIByIndex(byte index)
    {
        foreach (var tab in tabSwitchingUIs)
        {
            tab.Hide();
        }
        foreach (var tabBtn in buttonLists)
        {
            tabBtn.Interactable();
        }

        buttonLists[index].UnInteractable();
        tabSwitchingUIs[index].Show();
    }

}
