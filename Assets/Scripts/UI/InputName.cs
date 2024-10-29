using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InputName : MonoBehaviour
{
    public TMP_InputField nameInputField;
    public Button playButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetName()
    {
        GameManager.playerNickName = nameInputField.text;
        SceneManager.LoadScene("MainLobby");
    }
}
