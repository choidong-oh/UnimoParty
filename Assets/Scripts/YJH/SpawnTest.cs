using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SpawnTest : MonoBehaviourPun
{
    [System.Serializable]
    public class SpawnData
    {
        public string prefabName;
        public int count = 10;
    }

    [SerializeField]
    private List<SpawnData> spawnList;

    [Header("스폰 영역")]
    [SerializeField] private Vector3 areaCenter = Vector3.zero;
    [SerializeField] private Vector3 areaSize = new Vector3(50f, 0f, 50f);


    private bool isGameEnded = false;
    private Coroutine spawnRoutine;
    private void Start()
    {
        if (PhotonNetwork.PrefabPool == null)
            PhotonNetwork.PrefabPool = new DefaultPool();

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(SpawnAllMonsters());
        }
    }

    private void OnEnable()
    {
        Manager.Instance.observer.OnGameEnd += OnGameEndHandler;
    }

    private void OnDisable()
    {
        Manager.Instance.observer.OnGameEnd -= OnGameEndHandler;
    }
    private void OnGameEndHandler()
    {
        isGameEnded = true;

        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);
    }
    private IEnumerator SpawnAllMonsters()
    {
        foreach (var data in spawnList)
        {
            for (int i = 0; i < data.count; i++)
            {
                if (isGameEnded)
                    yield break;

                Vector3 randomPos = GetRandomSpawnPosition();
                photonView.RPC("RPC_SpawnMonster", RpcTarget.All, data.prefabName, randomPos, Quaternion.identity);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    [PunRPC]
    private void RPC_SpawnMonster(string prefabName, Vector3 position, Quaternion rotation)
    {
        PhotonNetwork.InstantiateRoomObject(prefabName, position, rotation);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 offset = new Vector3(
            Random.Range(-areaSize.x / 2, areaSize.x / 2),
            0,
            Random.Range(-areaSize.z / 2, areaSize.z / 2)
        );
        return areaCenter + offset;
    }
}
