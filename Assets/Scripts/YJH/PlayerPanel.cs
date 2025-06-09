using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour
{
    public int actorNumber;

    public Image skin;

    public TextMeshProUGUI nicknameText;
    public GameObject readyIcon;
    public GameObject masterIcon;

    public void Setup(Player photonPlayer)
    {
        actorNumber = photonPlayer.ActorNumber;
        nicknameText.text = photonPlayer.NickName;
        SetReady(false);
    }
    public void SetReady(bool isReady)
    {
        readyIcon.SetActive(isReady);
    }

    public void MasterClient(bool isMaster)
    {
        masterIcon.SetActive(isMaster);
    }

    public void SetNickname(string nickname)
    {
        nicknameText.text = nickname;
    }

    

    

}
