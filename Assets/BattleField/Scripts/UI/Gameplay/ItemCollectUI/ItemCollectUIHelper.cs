using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemCollectUIHelper : MonoBehaviour
{
    private ItemCollectionUI ItemCollectionUI;

    private float timer = 0;
    private float defaultCollectTimer = .5f;
    private bool canCollect = false;
    [SerializeField] private TextMeshProUGUI informationText;
    [SerializeField] private bool allowCollectByInput = true;

    private void Awake()
    {
        ItemCollectionUI = GetComponent<ItemCollectionUI>();
        InputCombatControl.GetInCar += GetInCar_performed;

#if PLATFORM_ANDROID 
        informationText.gameObject.SetActive(false);
#endif

    }

    private void OnDestroy()
    {
        InputCombatControl.GetInCar -= GetInCar_performed;
    }

    private void GetInCar_performed()
    {
        Collect();
    }

    [Button]
    private void Collect()
    {
        ItemCollectionUI.GetFirstItemDebug();
        canCollect = false;
        timer = defaultCollectTimer;
    }


    private void Update()
    {

        if (allowCollectByInput == false) return;

        if (canCollect == false)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                canCollect = true;
                timer = 0;
            }
        }
    }


}
