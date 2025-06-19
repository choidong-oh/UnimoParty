using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool;

[System.Serializable]
public class PoolItem
{
    [Tooltip("풀링을 적용할 프리팹")]
    public GameObject prefab;
    [Tooltip("풀 초기 용량")]
    public int defaultCapacity = 10;
    [Tooltip("풀 최대 크기")]
    public int maxSize = 50;
}

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [SerializeField] private List<PoolItem> poolItems;

    private Dictionary<GameObject, ObjectPool<GameObject>> pools;

    private Dictionary<GameObject, ObjectPool<GameObject>> instanceToPool;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        pools = new Dictionary<GameObject, ObjectPool<GameObject>>();
        instanceToPool = new Dictionary<GameObject, ObjectPool<GameObject>>();

        foreach (var item in poolItems)
        {
            var prefab = item.prefab;
            var pool = new ObjectPool<GameObject>(
                createFunc: () => { var go = Instantiate(prefab); go.SetActive(false); return go; },
                actionOnGet: go => { },      
                actionOnRelease: go => go.SetActive(false),
                actionOnDestroy: go => Destroy(go),
                collectionCheck: false,
                defaultCapacity: item.defaultCapacity,
                maxSize: item.maxSize
            );
            pools.Add(prefab, pool);
        }
    }

    public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        if (!pools.TryGetValue(prefab, out var pool))
            return Instantiate(prefab, pos, rot);

        //Debug.Log(prefab +" "+ pos +" "+ rot);
        var go = pool.Get();

        instanceToPool[go] = pool;

        go.transform.SetPositionAndRotation(pos, rot);
        go.SetActive(true);

        return go;
    }

    public void Despawn(GameObject instance)
    {
        if (instanceToPool.TryGetValue(instance, out var pool))
        {
            pool.Release(instance);
            instanceToPool.Remove(instance);
        }
        else
        {
            Destroy(instance);
        }
    }
}
