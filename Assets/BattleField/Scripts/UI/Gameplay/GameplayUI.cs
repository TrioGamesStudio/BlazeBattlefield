using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayUI : MonoBehaviour
{
    public static GameplayUI instance;
    public CanvasGroup CanvasGroup;

    private void Awake()
    {
        instance = this;
        CanvasGroup = GetComponent<CanvasGroup>();
        CanvasGroup.alpha = 1;
    }

}
