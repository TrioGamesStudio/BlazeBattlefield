using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputfieldHandler : MonoBehaviour
{
    public TextMeshProUGUI placeholderText;   
    public TMP_InputField inputField;
    private string originalText;

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
}
