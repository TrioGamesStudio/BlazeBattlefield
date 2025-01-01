using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class CharacterHatHandler : MonoBehaviour
{
    public static CharacterHatHandler instance;
    [SerializeField] private HatDataHandler hatDataHandler;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public GameObject GetHatPrefabByIndex(int hatIndex)
    {
        return hatDataHandler.hatsList[hatIndex];
    }
}
