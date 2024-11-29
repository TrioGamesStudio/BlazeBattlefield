using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIListManager : MonoBehaviour
{
    public GameObject uiPrefab; 
    public Transform container; 
    public float offsetY = 50f;
    private void Awake()
    {
        uiPrefab.gameObject.SetActive(false);
    }
    private List<GameObject> uiElements = new List<GameObject>();
    [Button]
    public void AddNewUI()
    {
        GameObject newUI = Instantiate(uiPrefab, container);

        uiElements.Add(newUI);
        newUI.gameObject.SetActive(true);
        
        UpdateUIPositions();
    }
    [Button]
    private void UpdateUIPositions()
    {
        int index = 0;
        for (int i = uiElements.Count-1; i >= 0; i--)
        {
            RectTransform rectTransform = uiElements[i].GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, index * offsetY);
            index++;
        }
    }
    


}
