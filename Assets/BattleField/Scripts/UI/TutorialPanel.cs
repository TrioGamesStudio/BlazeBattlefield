using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPanel : MonoBehaviour
{
    public GameObject[] tutorialViews;

    public void ActivePanel(int index)
    {
        for (int i = 0; i < tutorialViews.Length; i++)
        {
            if (i == index)
            {
                tutorialViews[i].gameObject.SetActive(true);
            }
            else
            {
                tutorialViews[i].gameObject.SetActive(false);
            }
        }
    }
}
