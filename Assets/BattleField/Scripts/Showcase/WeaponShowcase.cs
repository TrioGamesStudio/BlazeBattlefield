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
    private void Awake()
    {
        GoNextGun();
    }
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
        currentItem.transform.localScale = Vector3.one * 1.5f;
    }
    public float rotateSpeed = 2;
    private void Update()
    {
        if (currentItem == null) return;

        //float rotationX = weaponShowcaseUI.rotateInput.y * rotationSpeed;
        //float rotationY = -weaponShowcaseUI.rotateInput.x * rotationSpeed;

        //currentItem.transform.Rotate(Vector3.up, rotationY, Space.World);
        //currentItem.transform.Rotate(Vector3.right, rotationX, Space.Self);
        currentItem.transform.rotation = Quaternion.Lerp(currentItem.transform.rotation, 
            Quaternion.Euler(new Vector3(0, -weaponShowcaseUI.rotateInput.x, weaponShowcaseUI.rotateInput.y)), rotateSpeed * Time.deltaTime);
    }
}
