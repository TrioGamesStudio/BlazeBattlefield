using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShowcase : MonoBehaviour
{
    public List<GameObject> itemShowcase = new();
    public int index;
    private GameObject currentItem;
    public WeaponShowcaseUI weaponShowcaseUI;
    public float rotationSpeed = 5;
    [SerializeField] private Transform SpawnTransform;
    [Button]
    public void GoNextGun()
    {
        index++;
        if(index >= itemShowcase.Count)
        {
            index = 0;
        }
        if(currentItem != null)
            Destroy(currentItem.gameObject);
        currentItem = Instantiate(itemShowcase[index], SpawnTransform.position, Quaternion.identity);

    }

    private void Update()
    {
        if (currentItem == null) return;

        float rotationX = weaponShowcaseUI.rotateInput.y * rotationSpeed;
        float rotationY = -weaponShowcaseUI.rotateInput.x * rotationSpeed;

        currentItem.transform.Rotate(Vector3.up, rotationY, Space.World);
        currentItem.transform.Rotate(Vector3.right, rotationX, Space.Self);
    }
}
