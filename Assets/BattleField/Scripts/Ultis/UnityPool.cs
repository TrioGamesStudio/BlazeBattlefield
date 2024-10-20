using UnityEngine;
using UnityEngine.Pool;
public class UnityPool<PoolObj> where PoolObj : MonoBehaviour, IPoolCallback<PoolObj>
{
    protected IObjectPool<PoolObj> pool;
    protected PoolObj prefab;
    protected Transform parent;
    public UnityPool(PoolObj prefab, int size = 5,Transform parent = null)
    {
        if(prefab == null)
        {
            Debug.LogWarning("Prefab is null: Pool Obj Type " + typeof(PoolObj));
            return;
        }

        this.parent = parent;
        this.prefab = prefab;
        pool = new ObjectPool<PoolObj>(OnCreate, OnGet, OnRelease, OnDestroy, true, size,20);
    }
    public virtual PoolObj Get()
    {
        var poolObj = pool.Get();
        poolObj.OnCallback = (_poolObj) =>
        {
            pool.Release(_poolObj);
        };
        return poolObj;
    }

    protected virtual void OnDestroy(PoolObj obj)
    {
        Object.Destroy(obj.gameObject);
    }

    protected virtual void OnRelease(PoolObj obj)
    {
        obj.gameObject.SetActive(false);
    }
    
    protected virtual void OnGet(PoolObj obj)
    {
        obj.gameObject.SetActive(true);
    }

    protected virtual PoolObj OnCreate()
    {
        var obj = Object.Instantiate(prefab, parent);

        return obj;
    }
    public void Clear()
    {
        pool.Clear();
    }
}