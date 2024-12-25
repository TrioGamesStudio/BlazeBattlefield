using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenUI : MonoBehaviour
{
    public static LoadingScreenUI instance;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI sceneEnterName;
    [SerializeField] private TextMeshProUGUI mapName;
    private void Awake()
    {
        if(instance == null)
            instance = this;
        slider = GetComponentInChildren<Slider>();
        Hide();
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);

    public void Loading(AsyncOperation sceneOperation)
    {
        StartCoroutine(LoadSceneCoroutine(sceneOperation));
    }

    private IEnumerator LoadSceneCoroutine(AsyncOperation sceneOperation)
    {
        Show();
        while (!sceneOperation.isDone)
        {
            slider.value = Mathf.Clamp01(sceneOperation.progress / 0.9f);
            yield return null;
        }
        Hide();
    }
}
