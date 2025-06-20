using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    [Header("스폰 포인트 리스트 (인스펙터에서 수동으로 설정)")]
    public List<Transform> spawnPoints = new List<Transform>();

    private void Start()
    {
        StartCoroutine(wait());
        Debug.Log($"받은 캐릭터 인덱스: {SelectedData.characterIndex}");
        Debug.Log($"받은 우주선 인덱스: {SelectedData.shipIndex}");
    }

    IEnumerator wait()
    {
        yield return new WaitForSeconds(0.1f);
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            yield return new WaitForSeconds(0.1f);
            SpawnAtIndex(PhotonNetwork.LocalPlayer.ActorNumber - 1);
        }
    }

    public void SpawnAtIndex(int index)
    {
        if (index >= 0 && index < spawnPoints.Count)
        {
            Transform spawnPoint = spawnPoints[index];
            Quaternion yRotationOnly = Quaternion.Euler(0, spawnPoint.rotation.eulerAngles.y, 0);

            GameObject player = PhotonNetwork.Instantiate("PlayerVariant", spawnPoint.position, yRotationOnly);

            if (player.GetComponent<PhotonView>().IsMine)
            {
                PhotonView pv = player.GetComponent<PhotonView>();
                pv.RPC("SetupPlayer", RpcTarget.AllBuffered, SelectedData.characterIndex, SelectedData.shipIndex);
            }
        }
    }

}
