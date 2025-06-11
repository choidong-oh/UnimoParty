using UnityEngine;

public class TEST : MonoBehaviour
{
    public GameObject cubePrefab;
    private GameObject previewCube;

    public bool IsTuch = false;
    public bool tuch2 = false;

    void Update()
    {
        if (IsTuch == true)
        {
            if (previewCube == null)
            {
                previewCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Destroy(previewCube.GetComponent<Collider>());
                SetTransparent(previewCube, 0.3f);
            }

            Vector3 spawnPos = transform.position + transform.forward * 2f;
            previewCube.transform.position = spawnPos;
        }
        else if (IsTuch == false)
        {
            Destroy(previewCube);
        }

        if (tuch2 == true)
        {
            tuch2 = false;
            Vector3 spawnPos = transform.position + transform.forward * 2f;
            Instantiate(cubePrefab, spawnPos, Quaternion.identity);
        }
    }

    void SetTransparent(GameObject obj, float alpha)
    {
        var renderer = obj.GetComponent<MeshRenderer>();

        var mat = renderer.materials[0];

        mat.color = new Color(1, 1, 1, alpha);

        renderer.material = mat;
    }



}









