using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemCollectUIHelper : MonoBehaviour
{
    private ItemCollectionUI ItemCollectionUI;
    private PlayerInputAction playerInputActions;

    private float timer = 0;
    private float defaultCollectTimer = .5f;
    private bool canCollect = false;
    [SerializeField] private TextMeshProUGUI informationText;
    [SerializeField] private bool allowCollectByInput = true;

    private void Awake()
    {
        ItemCollectionUI = GetComponent<ItemCollectionUI>();
        playerInputActions = new PlayerInputAction();
        playerInputActions.Combat.Enable();
        playerInputActions.Combat.GetInCar.performed += GetInCar_performed;

#if PLATFORM_ANDROID 
        informationText.gameObject.SetActive(false);
#endif

    }

    private void OnDestroy()
    {
        playerInputActions.Combat.GetInCar.performed -= GetInCar_performed;
    }

    private void GetInCar_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
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
