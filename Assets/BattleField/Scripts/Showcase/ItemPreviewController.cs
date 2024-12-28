using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class ItemPreviewController : MonoBehaviour
{
    [Header("References")]
    public Transform itemContainer; // Container cho vật phẩm
    public Transform itemModel;     // Model của vật phẩm

    [Header("Rotation Settings")]
    public float rotationSpeed = 100f;
    public Vector2 verticalRotationLimits = new Vector2(-80f, 80f);

    private float currentVerticalRotation = 0f;
    private Vector3 initialModelRotation;

    void Start()
    {
        // Lưu lại rotation ban đầu của model
        if (itemModel != null)
        {
            initialModelRotation = itemModel.localEulerAngles;
        }

        // Reset container về rotation 0
        itemContainer.localEulerAngles = Vector3.zero;
    }

    

    public void SetNewItem(GameObject newItem)
    {
        // Xóa item cũ nếu có
        if (itemModel != null)
        {
            Destroy(itemModel.gameObject);
        }

        // Tạo item mới
        GameObject spawnedItem = Instantiate(newItem, itemContainer);
        itemModel = spawnedItem.transform;

        // Reset về trạng thái ban đầu
        currentVerticalRotation = 0f;
        initialModelRotation = itemModel.localEulerAngles;
        itemContainer.localEulerAngles = Vector3.zero;

        // Đặt vị trí item về center của container
        itemModel.localPosition = Vector3.zero;
    }
}
