using System.Collections.Generic;
using UnityEngine;

public class LerpManager : MonoBehaviour
{
    private static LerpManager instance;
    public static LerpManager Instance
    {
        get
        {
            if (instance == null)
            {
                var current = new GameObject();
                current.name = "LerpManager";
                DontDestroyOnLoad(current);
                instance = current.AddComponent<LerpManager>();
            }
            return instance;
        }
    }

    [SerializeField] private List<LerpValue> lerpValues = new();
    private List<LerpValue> lerpValuesToAdd = new();
    private List<LerpValue> lerpValuesToRemove = new();

    public void AddToLerp(LerpValue lerpValue)
    {
        if (!lerpValues.Contains(lerpValue) && !lerpValuesToAdd.Contains(lerpValue))
        {
            lerpValuesToAdd.Add(lerpValue);
        }
    }

    public void RemoveToLerp(LerpValue lerpValue)
    {
        if (lerpValues.Contains(lerpValue) && !lerpValuesToRemove.Contains(lerpValue))
        {
            lerpValuesToRemove.Add(lerpValue);
        }
    }

    private void Update()
    {
        // Add new lerp values
        if (lerpValuesToAdd.Count > 0)
        {
            lerpValues.AddRange(lerpValuesToAdd);
            lerpValuesToAdd.Clear();
        }

        // Update existing lerp values
        foreach (var item in lerpValues)
        {
            item.Update(Time.deltaTime);
        }

        // Remove lerp values
        if (lerpValuesToRemove.Count > 0)
        {
            foreach (var item in lerpValuesToRemove)
            {
                lerpValues.Remove(item);
            }
            lerpValuesToRemove.Clear();
        }
    }
}
