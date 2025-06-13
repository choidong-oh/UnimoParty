// PoolManager.cs
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
    public int maxSize = 100;
}

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [Header("여기에 풀링할 몬스터 프리팹들을 추가하세요")]
    [SerializeField] private List<PoolItem> poolItems;

    // 프리팹 → ObjectPool 매핑
    private Dictionary<GameObject, ObjectPool<GameObject>> pools;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        pools = new Dictionary<GameObject, ObjectPool<GameObject>>();
        foreach (var item in poolItems)
        {
            var prefab = item.prefab; // 로컬 복사
            var pool = new ObjectPool<GameObject>(
                // 생성 로직
                createFunc: () =>
                {
                    var go = Instantiate(prefab);
                    go.SetActive(false);
                    return go;
                },
                // Get() 시
                actionOnGet: go =>
                {
                    go.SetActive(true);
                },
                // Release() 시
                actionOnRelease: go =>
                {
                    go.SetActive(false);
                },
                // 풀 초과 시 파괴
                actionOnDestroy: go =>
                {
                    Destroy(go);
                },
                collectionCheck: false,
                defaultCapacity: item.defaultCapacity,
                maxSize: item.maxSize
            );

            pools.Add(prefab, pool);
        }
    }

    /// <summary>
    /// 원하는 프리팹으로부터 풀에서 꺼내 스폰합니다.
    /// </summary>
    public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        if (!pools.TryGetValue(prefab, out var pool))
        {
            Debug.LogWarning($"PoolManager: 풀에 등록되지 않은 prefab({prefab.name}) 입니다. Instantiate 사용.");
            return Instantiate(prefab, pos, rot);
        }

        var go = pool.Get();
        go.transform.SetPositionAndRotation(pos, rot);


        var bd = go.GetComponent<Burnduri>();
        if (bd != null)
            bd.prefab = prefab;


        return go;
    }

    /// <summary>
    /// 스폰된 오브젝트를 다시 풀에 반환합니다.
    /// </summary>
    public void Despawn(GameObject prefab, GameObject instance)
    {
        if (pools.TryGetValue(prefab, out var pool))
        {
            Debug.Log("풀링 잘작동한듯?");
            Debug.Log(prefab);
            pool.Release(instance);
        }
        else
        {
            Debug.Log("파괴!");
            Debug.Log(prefab);
            Destroy(instance);
        }
    }
}
