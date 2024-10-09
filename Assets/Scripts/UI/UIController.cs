using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIController : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject panelMode;        
    public GameObject panelTeamJoin;
    public Button showButton;       
    public GameObject background;
    public Image modeImage;
    public Sprite singleMode;
    public Sprite duoMode;
    private bool isPanelActive = false;
    //bool isSingle = true;

    void Start()
    {
        InitializeUI();
    }

    void Update()
    {
        HandleTouchInput();
    }

    // Initializes the button listeners and ensures the panel is hidden initially
    private void InitializeUI()
    {
        HidePanel();

        if (showButton != null)
        {
            // Assign the button's onClick event to toggle the panel
            showButton.onClick.AddListener(ShowPanel);
        }
    }

    // Handles touch input to hide the panel if the touch is on the background but not on the button
    private void HandleTouchInput()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (IsTouchOnBackgroundOnly())
            {
                HidePanel();
            }
        }
    }

    private bool IsTouchOnBackgroundOnly()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = Input.GetTouch(0).position;
        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        // Check if the only hit UI element is the background
        return results.Count == 1 && results[0].gameObject == background;

        //// Check if the touch is over any UI element
        //PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        //eventDataCurrentPosition.position = Input.GetTouch(0).position;
        //List<RaycastResult> results = new();
        //EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        //return results.Count > 0;
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
        if (isPanelActive) return;
        panelMode.SetActive(true);
        isPanelActive = true;
    }

    // Method to hide the panel and set it inactive
    public void HidePanel()
    {
        panelMode.SetActive(false);
        panelTeamJoin.SetActive(false);
        isPanelActive = false;
    }

    public void SwitchMode(bool single)
    {
        if (single)
            modeImage.sprite = singleMode;
        else
            modeImage.sprite = duoMode;
        //isSingle = !isSingle;
    }

    public void ShowHidePanel(GameObject panelUI)
    {
        if (panelUI.activeSelf == false)
        {
            panelUI.SetActive(true);
        }
    }
}
