using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using Firebase.Extensions;

public class LoginManager : MonoBehaviour
{
    private FirebaseAuth auth;
    public TextMeshProUGUI textInformation;

    void Start()
    {
        // Initialize FirebaseAuth
        auth = FirebaseAuth.DefaultInstance;

        // Start Anonymous Login
        //LoginAnonymously();
    }

    public void LoginAnonymously()
    {
        auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                AuthResult user = task.Result;
                Debug.Log($"Anonymous login successful! UserID: {user.AdditionalUserInfo}");
                textInformation.text = "Anonymous login successful!";
                Invoke(nameof(HandleLoginSuccess), 1f);
            }
            else
            {
                Debug.LogError($"Anonymous login failed: {task.Exception}");
                textInformation.text = $"Anonymous login failed: {task.Exception}";
            }
        });
        //auth.SignInWithCredentialAsync().Co
    }

    void HandleLoginSuccess()
    {
        
        SceneManager.LoadScene("MainLobby");
    }

}
