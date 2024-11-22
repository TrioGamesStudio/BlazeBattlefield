using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InputfieldHandler : MonoBehaviour
{
    public TextMeshProUGUI placeholderText;   
    public TMP_InputField inputField;
    private string originalText;

    public Button toggleButton;          // Reference to the toggle button
    public Sprite showPasswordIcon;      // Icon for "show password"
    public Sprite hidePasswordIcon;      // Icon for "hide password"
    private bool isPasswordHidden = true; // Track whether the password is hidden


    void Start()
    {
        originalText = placeholderText.text;
    }

    public void OnInputFieldSelected()
    {
        ScaleUp();
        if (placeholderText != null)
            placeholderText.text = "";

    }

    public void OnInputFieldDeselected()
    {
        ScaleDown();
        if (placeholderText != null && string.IsNullOrEmpty(inputField.text))
        {
            placeholderText.text = originalText;
        }        
    }

    void ScaleUp()
    {
        inputField.gameObject.transform.localScale = Vector3.one * 1.1f;
    }

    void ScaleDown()
    {
        inputField.gameObject.transform.localScale = Vector3.one;
    }

    public void TogglePasswordVisibility()
    {      
        UpdatePasswordVisibility();
        // Toggle the visibility state
        isPasswordHidden = !isPasswordHidden;
    }

    void UpdatePasswordVisibility()
    {
        // Update the input field's content type and refresh it
        if (isPasswordHidden)
        {
            inputField.contentType = TMP_InputField.ContentType.Standard;
            toggleButton.image.sprite = showPasswordIcon; // Set the "show password" icon
        }
        else
        {
            inputField.contentType = TMP_InputField.ContentType.Password;
            toggleButton.image.sprite = hidePasswordIcon; // Set the "hide password" icon
        }

        inputField.ForceLabelUpdate(); // Refresh the input field to apply changes
    }
}
