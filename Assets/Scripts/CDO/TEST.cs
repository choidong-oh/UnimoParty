using UnityEngine;
using Photon.Pun;
using TMPro;


public class TEST : MonoBehaviour, IItemUse
{
    [SerializeField] Material material;
    [SerializeField] int hp;

    public bool Use(Transform firePos, int power)
    {
        hp = Manager.Instance.observer.UserPlayer.gamedata.life;
        Debug.Log("player hp = " + hp);
        Manager.Instance.observer.RecoveryPlayerHP(10);



        hp = Manager.Instance.observer.UserPlayer.gamedata.life;
        Debug.Log("player hp = " + hp);

        //PhotonNetwork.Destroy(gameObject);  
        return true;
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









