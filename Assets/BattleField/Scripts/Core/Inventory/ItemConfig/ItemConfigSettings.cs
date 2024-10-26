using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemConfigSettings<T,F> : ScriptableObject where T : Enum where F : ItemConfig<T>
{
    public List<F> itemConfigs;
}
[Serializable]
public class ItemConfig<T> where T : Enum
{
    public Sprite Icon;
    public T ItemType;
    public int maxStack;
}
