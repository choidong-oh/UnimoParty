using UnityEngine;
using Photon.Pun;
using TMPro;
using System.Collections;


public class TEST : MonoBehaviourPunCallbacks, IItemUse
{
    [SerializeField] Material material;
    [SerializeField] int hp;

    public bool Use(Transform firePos, int power)
    {
        StartCoroutine(UseWait());

        return true;
    }

    
    IEnumerator UseWait()
    {
        Manager.Instance.observer.RecoveryPlayerHP(10);
        yield return null;

        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }


    private void Awake()
    {
        var renderer = GetComponent<MeshRenderer>();
        var materials = renderer.materials;

        materials[0] = material;
        renderer.materials = materials;
    }

    private void Start()
    {
        hp = Manager.Instance.observer.UserPlayer.gamedata.life;
    }


}









