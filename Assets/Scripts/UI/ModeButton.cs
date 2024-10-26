using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeButton : MonoBehaviour
{
    public Button singleButton;         // Single mode button
    public GameObject singleHighlight;  // Highlight component for Single button
    // Start is called before the first frame update
    void Start()
    {
        InitializeUI();
    }

    // Initializes the button listeners and ensures the panel and highlights are set properly
    private void InitializeUI()
    {
        if (singleButton != null)
        {
            singleButton.onClick.AddListener(OnSingleButtonClicked);
        }
    }

    // Event handler for the Single button click
    private void OnSingleButtonClicked()
    {
        Highlight();
    }

    // Highlight the Single button and deactivate Duo's highlight
    public void Highlight()
    {
        singleHighlight.SetActive(true);
    }

    public void UnHightLight()
    {
        singleHighlight.SetActive(false);
    }
}
