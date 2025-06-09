using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class PlayerPanel : MonoBehaviour
{
    public int actorNumber;

    public TextMeshProUGUI nicknameText;
    public GameObject readyIcon;
    public GameObject masterIcon;
    public void SetNickname(string nickname)
    {
        nicknameText.text = nickname;
    }
    public void MasterClient(bool isMaster)
    {
        masterIcon.SetActive(isMaster);
    }
    public void SetReady(bool isReady)
    {
        readyIcon.SetActive(isReady);
    }
    public void Setup(Player photonPlayer)
    {
        actorNumber = photonPlayer.ActorNumber;
        nicknameText.text = photonPlayer.NickName;
        SetReady(false);
    }
}
