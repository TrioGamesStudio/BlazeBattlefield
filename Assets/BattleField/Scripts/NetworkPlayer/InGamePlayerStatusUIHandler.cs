using TMPro;
using UnityEngine;

public class InGamePlayerStatusUIHandler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI playerHP;
    [SerializeField] HPHandler HPHandler;
    private void Awake()
    {
        HPHandler = GetComponentInParent<HPHandler>();
        HPHandler.OnHpChanged += OnHPChanged;
    }
    private void OnDestroy()
    {
        HPHandler.OnHpChanged -= OnHPChanged;
    }

    private void OnHPChanged()
    {
        OnGamePlayerHpRecieved(HPHandler.Networked_HP);
    }

    public void OnGamePlayerHpRecieved(byte hp) {
        playerHP.text = $"HP: {hp.ToString()}" ;
    }
}
