using Fusion;
using UnityEngine;
public class TagObjectHandler : NetworkBehaviour, IAfterSpawned
{
    [Networked] public string ObjectTag { get; set; }

    public void AfterSpawned()
    {
        if (string.IsNullOrEmpty(ObjectTag))
        {
            ObjectTag = "Item";
        }
        tag = ObjectTag;
        Debug.Log("Tag assign is: " + ObjectTag);
    }
}