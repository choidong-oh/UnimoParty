using UnityEngine;

public class Barricade : MonoBehaviour, IItemUse
{
    [SerializeField] GameObject realBarricadPrefab; //실제 생성
    GameObject previewBarricadPrefab; //미리보기

    //[SerializeField] Transform rightControllerPos; //플레이어 
    Vector3 previewPlayerPos;
    Bounds previewBounds;

    public LayerMask noGroundlayerMask; //그라운드만 빼고 체크

    public bool bool2 = false;

    bool isBlocked; //겹치는 오브젝트가있냐? 콜라이더되는애들이 있냐

    private MeshRenderer previewRenderer;
    private void OnEnable()
    {
        if (previewBarricadPrefab == null)
        {
            previewBarricadPrefab = Instantiate(realBarricadPrefab);
            Destroy(previewBarricadPrefab.GetComponent<Collider>());
            GroundPos();

            previewRenderer = previewBarricadPrefab.GetComponent<MeshRenderer>();

        }
    }

    private void OnDisable()
    {
        DestotyPreviewPrefab();
    }



    private void FixedUpdate()
    {
        if (previewBarricadPrefab == null)
        {
            return;
        }

        //바닥체크 및 움직임
        GroundPos();

        previewBounds = previewRenderer.bounds;

        isBlocked = Physics.CheckBox(previewBounds.center,
                                     previewBounds.extents,
                                     previewBarricadPrefab.transform.rotation,
                                     noGroundlayerMask);
        if (isBlocked == true)
        {
            Debug.Log("겹치는 오브젝트 있음요");
            SetTransparent(0.3f, Color.red);
        }
        else
        {
            SetTransparent(0.3f, Color.green);
        }


    }

    //미리보기 아이템 삭제
    void DestotyPreviewPrefab()
    {
        if (previewBarricadPrefab != null)
        {
            Destroy(previewBarricadPrefab);
            previewBarricadPrefab = null;
        }

    }



    //바닥 포지션
    Vector3 GroundPos()
    {
        var tempPos = transform.position + transform.forward * 5f;

        if (Physics.Raycast(tempPos, Vector3.down, out RaycastHit hit, 10f))
        {
            if (hit.collider.gameObject.tag == "Ground")
            {



                //포지션
                Vector3 spawnPos = hit.point;
                spawnPos.y += previewBounds.extents.y;
                var tempSpawnPos = spawnPos.y;

                spawnPos += transform.forward * 5f;
                previewBarricadPrefab.transform.position = spawnPos;

                //로테이션
                Vector3 direction = transform.position - previewBarricadPrefab.transform.position;
                direction.y = 0;

                Quaternion rotation = Quaternion.LookRotation(direction);

                previewBarricadPrefab.transform.rotation = rotation;

                previewBarricadPrefab.transform.position = new Vector3(spawnPos.x, tempSpawnPos, spawnPos.z);



                previewPlayerPos = previewBarricadPrefab.transform.position;

            }
        }


        return previewPlayerPos;
    }

    //머티리얼 색변환
    void SetTransparent(float alpha, Color color)
    {
        if (previewRenderer == null)
        {
            return;
        }

        var mat = previewRenderer.materials[0];

        color.a = alpha;
        mat.color = color;
        previewRenderer.material = mat;
    }



    //설치 가능한지
    bool CanPlace()
    {
        if (previewBarricadPrefab == null) { return false; }

        isBlocked = Physics.CheckBox(previewBounds.center,
                                     previewBounds.extents,
                                     previewBarricadPrefab.transform.rotation,
                                     noGroundlayerMask);

        return isBlocked;
    }

    public void Use(Transform firePos, int power)
    {

        if (CanPlace() == false)
        {
            Instantiate(realBarricadPrefab, GroundPos(), Quaternion.identity);
            DestotyPreviewPrefab();
        }
        else
        {
            Debug.Log("겹쳐서 배치 불가!");
        }



    }
}
