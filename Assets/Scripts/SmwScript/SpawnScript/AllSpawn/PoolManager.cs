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
            var prefab = item.prefab;
            var pool = new ObjectPool<GameObject>(

                createFunc: () =>
                {
                    var go = Instantiate(prefab);
                    go.SetActive(false);
                    return go;
                },
                actionOnRelease: go =>
                {
                    go.SetActive(false);
                },

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

    public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        if (!pools.TryGetValue(prefab, out var pool))
            return Instantiate(prefab, pos, rot);

        // (1) Get은 비활성 상태로 반환
        var go = pool.Get();

            // (2) 위치/회전 세팅
             go.transform.SetPositionAndRotation(pos, rot);

            // (3) 여기서 활성화 → OnEnable()이 제대로 pos를 읽음
             go.SetActive(true);

        var bd = go.GetComponent<Burnduri>();
        if (bd != null)
            bd.prefab = prefab;

        return go;
    }

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
