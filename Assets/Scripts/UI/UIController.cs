using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
//using UnityEngine.UIElements;
//using Image = UnityEngine.UI.Image;
//using Button = UnityEngine.UI.Button;

public class UIController : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject panel;        // The panel to show/hide
    public Button showButton;       // Button to show/hide the panel
    public GameObject background;   // Background UI element (Image or similar)
    public Image modeImage;
    public Sprite singleMode;
    public Sprite duoMode;
    private bool isPanelActive = false;
    bool isSingle = true;

    void Start()
    {
        InitializeUI();
    }

    void Update()
    {
        //HandleTouchInput();
    }

    // Initializes the button listeners and ensures the panel is hidden initially
    private void InitializeUI()
    {
        HidePanel();

        if (showButton != null)
        {
            // Assign the button's onClick event to toggle the panel
            showButton.onClick.AddListener(TogglePanel);
        }
    }

    // Handles touch input to hide the panel if the touch is on the background but not on the button
    private void HandleTouchInput()
    {
        if (isPanelActive && Input.touchCount > 0)
        {
            // Ignore if touch is on a UI element like the showButton
            if (!IsPointerOverUI() && IsTouchOnBackground())
            {
                HidePanel();
            }
        }
    }

    // Toggles the panel between show and hide states
    private void TogglePanel()
    {
        if (isPanelActive)
        {
            HidePanel();
        }
        else
        {
            ShowPanel();
        }
    }

    // Method to show the panel and set it active
    public void ShowPanel()
    {
        panel.SetActive(true);
        isPanelActive = true;
    }

    // Method to hide the panel and set it inactive
    public void HidePanel()
    {
        panel.SetActive(false);
        isPanelActive = false;
    }

    // Utility method to check if the touch is on the background UI element
    private bool IsTouchOnBackground()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = Input.GetTouch(0).position
        };

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, raycastResults);

        foreach (RaycastResult result in raycastResults)
        {
            // Check if the touched object is the background
            if (result.gameObject == background)
            {
                return true;
            }
        }
        return false;
    }

    // Method to check if the touch is over any UI element (like the showButton)
    private bool IsPointerOverUI()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = Input.GetTouch(0).position
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        // If results contain any UI element, the touch is over UI
        return results.Count > 0;
    }

    public void SwitchMode(bool single)
    {
        if (single)
            modeImage.sprite = singleMode;
        else
            modeImage.sprite = duoMode;
        //isSingle = !isSingle;
    }

    // Future expansion: Add additional methods for handling other input types, animations, etc.
    // For example: 
    // - AddPanelAnimation()
    // - HandleKeyboardInput()
    // - ShowDifferentUI()
}
