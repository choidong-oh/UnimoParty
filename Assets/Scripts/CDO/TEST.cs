using UnityEngine;



//bounds
//bounds.center 프리팹의 정중앙
//bounds.extents half프리팹의 2분의1의크기 xyz (반지름)
public class TEST : MonoBehaviour
{
    public GameObject cubePrefab;
    private GameObject previewCube;

    public bool bool1 = false;
    public bool bool2 = false;

    public LayerMask layerMask;

    void Update()
    {
        //아이템 미리보기
        if (bool1 == true)
        {
            if (previewCube == null)
            {
                previewCube = Instantiate(cubePrefab);
                Destroy(previewCube.GetComponent<Collider>());
                SetTransparent(previewCube, 0.3f, Color.black);
            }

            Vector3 Player = transform.position + transform.forward * 2f;

            if (Physics.Raycast(Player, Vector3.down, out RaycastHit hit, 10f))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    Bounds bounds = previewCube.GetComponent<MeshRenderer>().bounds;
                    Vector3 spawnPos = hit.point;
                    spawnPos.y += bounds.extents.y;
                    previewCube.transform.position = spawnPos;
                }
            }

            int groundLayer = LayerMask.GetMask("Water"); 
            int mask = ~groundLayer;//그라운드만 제외 비트연산
            // 나중에 레이어마스크 인스펙터창에서 빼야할듯 public LayerMask layerMask; 
            Bounds previewBounds = previewCube.GetComponent<Renderer>().bounds;

            bool isBlocked = Physics.CheckBox (previewBounds.center,
                                               previewBounds.extents,
                                               previewCube.transform.rotation,
                                               mask);

            if (isBlocked == true)
            {
                Debug.Log("겹치는 오브젝트 있음!");
                SetTransparent(previewCube, 0.3f, Color.red);
            }
            else
            {
                SetTransparent(previewCube, 0.3f, Color.green);
            }

        }
        else if (bool1 == false)
        {
            //기존 프리팹 삭제
            Destroy(previewCube);

        }



        //아이템 생성
        if (bool2 == true)
        {
            bool2 = false;

            if (CanPlace() == false)
            {
                Instantiate(cubePrefab, GroundPos(), Quaternion.identity);
            }
            else
            {
                Debug.Log("겹쳐서 배치 불가!");
            }
        }



    }

    //설치 가능한지
    bool CanPlace()
    {
        if (previewCube == null) { return false; }

        Bounds previewBounds = previewCube.GetComponent<Renderer>().bounds;

        bool isBlocked = Physics.CheckBox(previewBounds.center, previewBounds.extents, previewCube.transform.rotation);

        return isBlocked;
    }

    //바닥 포지션
    Vector3 GroundPos()
    {
        Vector3 Player = transform.position + transform.forward * 2f;

        if (Physics.Raycast(Player, Vector3.down, out RaycastHit hit, 10f))
        {
            if (hit.collider.gameObject.tag == "Ground")
            {
                Bounds bounds = previewCube.GetComponent<MeshRenderer>().bounds;
                Vector3 spawnPos = hit.point;
                spawnPos.y += bounds.extents.y;
                return spawnPos;
            }
        }

        return Player;
    }


    //머티리얼 색변환
    void SetTransparent(GameObject obj, float alpha, Color color)
    {
        var renderer = obj.GetComponent<MeshRenderer>();

        var mat = renderer.materials[0];

        color.a = alpha;
        mat.color = color;
        renderer.material = mat;
    }



}









