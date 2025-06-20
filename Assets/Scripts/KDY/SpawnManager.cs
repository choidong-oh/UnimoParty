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

    // 특정 인덱스 위치에 플레이어를 스폰하는 함수
    public void SpawnAtIndex(int index)
    {
        if (index >= 0 && index < spawnPoints.Count)
        {
            Transform spawnPoint = spawnPoints[index];
            Quaternion yRotationOnly = Quaternion.Euler(0, spawnPoint.rotation.eulerAngles.y, 0);

            GameObject player = PhotonNetwork.Instantiate("PlayerVariant", spawnPoint.position, yRotationOnly);

            if (player.GetComponent<PhotonView>().IsMine)
            {
                Debug.Log("여기 들어오냐?");
                StartCoroutine(SetUpCharacterAndShip(player));
            }
        }
    }

    IEnumerator SetUpCharacterAndShip(GameObject player)
    {
        yield return new WaitForSeconds(0.1f);

        Transform characterPos = player.transform.Find("XR Origin (XR Rig)/CharacterPos");
        Transform xrOrigin = player.transform.Find("XR Origin (XR Rig)");

        Transform shipPos = null;

        Transform spaceShip = xrOrigin.GetChild(3);
        shipPos = spaceShip.GetChild(1);

        Debug.Log(characterPos + " ← 캐릭터 포지션");
        Debug.Log(shipPos + " ← 우주선 포지션");

        GameObject[] characters = Resources.LoadAll<GameObject>("Characters");
        GameObject[] ships = Resources.LoadAll<GameObject>("Prefabs");

        GameObject charObj = Instantiate(characters[SelectedData.characterIndex], characterPos.position, Quaternion.identity, characterPos);
        charObj.transform.localPosition = Vector3.zero;

        GameObject shipObj = Instantiate(ships[SelectedData.shipIndex], shipPos.position, Quaternion.identity, shipPos);
        shipObj.transform.localPosition = Vector3.zero;

    }
}
