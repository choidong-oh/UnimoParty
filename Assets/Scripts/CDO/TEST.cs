using UnityEngine;
using Photon.Pun;


public class TEST : MonoBehaviour, IItemUse
{
    [SerializeField] Material material;

    public bool Use(Transform firePos, int power)
    {
        PhotonNetwork.Destroy(gameObject);  
        return true;
    }

    private void Awake()
    {
        var renderer = GetComponent<MeshRenderer>();
        var materials = renderer.materials;

        materials[0] = material;
        renderer.materials = materials;
    }




}









