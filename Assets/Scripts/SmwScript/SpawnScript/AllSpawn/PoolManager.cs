using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[System.Serializable]
public class PoolItem
{
    [Tooltip("풀링을 적용할 프리팹)")]
    public string prefabName;
    [Tooltip("풀 초기 용량")]
    public int defaultCapacity = 10;
    [Tooltip("풀 최대 크기")]
    public int maxSize = 50;
}

//public class PoolManager : MonoBehaviourPun
//{
//    public static PoolManager Instance { get; private set; }

//    [SerializeField]
//    private List<PoolItem> poolItems;

//    private Dictionary<string, ObjectPool<GameObject>> pools;
//    private Dictionary<GameObject, ObjectPool<GameObject>> instanceToPool;

//    private void Awake()
//    {
//        if (Instance != null && Instance != this)
//        {
//            Destroy(gameObject);
//            return;
//        }
//        Instance = this;

//        pools = new Dictionary<string, ObjectPool<GameObject>>();
//        instanceToPool = new Dictionary<GameObject, ObjectPool<GameObject>>();

//        foreach (var item in poolItems)
//        {
//            string key = item.prefabName;
//            var pool = new ObjectPool<GameObject>(
//                createFunc: () =>
//                {
//                    var go = PhotonNetwork.Instantiate(key, Vector3.zero, Quaternion.identity);
//                    go.SetActive(false);
//                    return go;
//                },
//                actionOnGet: go => { },
//                actionOnRelease: go => go.SetActive(false),
//                actionOnDestroy: go => PhotonNetwork.Destroy(go),
//                collectionCheck: false,
//                defaultCapacity: item.defaultCapacity,
//                maxSize: item.maxSize
//            );
//            pools.Add(key, pool);
//        }
//    }


//    public GameObject Spawn(string prefabName, Vector3 position, Quaternion rotation)
//    {
//        if (!pools.TryGetValue(prefabName, out var pool))
//        {
//            return PhotonNetwork.Instantiate(prefabName, position, rotation);
//        }

//        var go = pool.Get();
//        instanceToPool[go] = pool;

//        go.transform.SetPositionAndRotation(position, rotation);
//        go.SetActive(true);
//        return go;
//    }


//    public void Despawn(GameObject instance)
//    {
//        if (instanceToPool.TryGetValue(instance, out var pool))
//        {
//            pool.Release(instance);
//            instanceToPool.Remove(instance);
//        }
//        else
//        {
//            PhotonNetwork.Destroy(instance);
//        }
//    }
//}
public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public GameObject SpawnNetworked(string prefabName, Vector3 position, Quaternion rotation)
    {
        return PhotonNetwork.InstantiateRoomObject(prefabName, position, rotation);
    }

    public void DespawnNetworked(GameObject obj)
    {
        PhotonNetwork.Destroy(obj);
    }
}