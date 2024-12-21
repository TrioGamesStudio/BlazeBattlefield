using UnityEngine;

public class TabSwitchingUI : MonoBehaviour
{
    public Sprite TabIcon;
    public byte priority = 0;
    public byte order = 0;
    public byte index;

    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
