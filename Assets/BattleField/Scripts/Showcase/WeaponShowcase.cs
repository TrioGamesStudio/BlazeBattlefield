using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShowcase : MonoBehaviour
{
    [SerializeField] private GameObject currentItem;
    [SerializeField] private GameObject currentItemPrefab;
    [SerializeField] private Transform SpawnTransform;
    [SerializeField] private WeaponShowcaseUI weaponShowcaseUI;
    public Quaternion weaponQuaternion;
    public float rotateSpeed = 2;
    public Vector2 defaultRotateInput;
    private void Awake()
    {

    }
    public void SetItemPrefab(GameObject newItemPrefab)
    {
        currentItemPrefab = newItemPrefab;
    }

    public void CreateItem()
    {
        if (currentItem != null)
            Destroy(currentItem.gameObject);
        currentItem = Instantiate(currentItemPrefab, SpawnTransform.position, weaponQuaternion, SpawnTransform);
        currentItem.transform.localScale = Vector3.one * 1.5f;
    }

    private void Update()
    {
        if (weaponShowcaseUI == null) return;
        if (currentItem == null) return;

        var newPosition = defaultPosition + offsetPosition;
        var newRotation = defaultRotation + offsetRotation;
        currentItem.transform.localRotation = Quaternion.Lerp(currentItem.transform.localRotation,
            Quaternion.Euler(new Vector3(0, -weaponShowcaseUI.rotateInput.x, weaponShowcaseUI.rotateInput.y)), rotateSpeed * Time.deltaTime);
    }
    [SerializeField] private Vector3 defaultPosition;
    [SerializeField] private Vector3 defaultRotation;
    [SerializeField] private Vector3 offsetPosition;
    [SerializeField] private Vector3 offsetRotation;

    private void GetDefaultValue()
    {
        defaultPosition = SpawnTransform.position;
        float x = SpawnTransform.rotation.x;
        float y = SpawnTransform.rotation.y;
        float z = SpawnTransform.rotation.z;
        defaultRotation = new Vector3(x, y, z);
    }

    public void SetOffset(Vector3 _offsetPosition, Vector3 _offsetRotation)
    {
        offsetPosition = _offsetPosition;
        offsetRotation = _offsetRotation;
    }

}
