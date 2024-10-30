using UnityEngine;
using UnityEngine.Events;

public class ViewUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject[] viewObjects;
    [SerializeField] private UnityEvent UISetupEvent;
  
    public void RaiseSetupUI()
    {
        UISetupEvent?.Invoke();
    }

    public void Show()
    {
        SetAcitveGameOjbect(true);
    }

    public void Hide()
    {
        SetAcitveGameOjbect(false);
    }

    private void SetAcitveGameOjbect(bool state)
    {
        foreach (var item in viewObjects)
        {
            item.gameObject.SetActive(state);
        }
    }
}