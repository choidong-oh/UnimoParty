using UnityEngine;



public class TEST : MonoBehaviour
{
    [SerializeField] Material material;

    private void Awake()
    {
        var renderer = GetComponent<MeshRenderer>();
        var materials = renderer.materials;

        materials[0] = material;
        renderer.materials = materials;
    }




}









