using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;

public class Barricade : MonoBehaviourPunCallbacks, IItemUse, InterfaceMethod.IItemData
{
    [SerializeField] int installMaxDistance; //설치 거리

    //[SerializeField] GameObject realBarricadPrefab; //실제 생성
    GameObject previewBarricadPrefab; //미리보기

    Vector3 previewPlayerPos;
    Bounds previewBounds;

    public LayerMask noGroundlayerMask; //그라운드만 빼고 체크

    bool isBlocked; //겹치는 오브젝트가있냐? 콜라이더되는애들이 있냐

    private MeshRenderer previewRenderer;

    //그랩////////////////////////////////////////////////////////////
    private XRGrabInteractable grabInteractable;

    public bool isGrab = false;
    bool isOneGrab = false;
    float rotationY = 0;

    public ItemData ItemData { get; set; }

    public override void OnEnable()
    {
        base.OnEnable();
        if (previewBarricadPrefab == null)
        {
            previewBarricadPrefab = PhotonNetwork.Instantiate("Barricade",transform.position,Quaternion.identity);

            Destroy(previewBarricadPrefab.GetComponent<Collider>());
            GroundPos();

            previewRenderer = previewBarricadPrefab.GetComponent<MeshRenderer>();

        }
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        isGrab = false;
        isOneGrab = false
    }

    public override void OnDisable()
    {
        base.OnDisable();
        DestoryPreviewPrefab();
        grabInteractable.selectEntered.RemoveListener(OnGrab);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        Debug.Log("그랩햄");
        if (isOneGrab == false)
        {
            isOneGrab = true;
            isGrab = true;
        }
       
    }

    private void FixedUpdate()
    {
        if (previewBarricadPrefab == null || photonView.IsMine ==false )
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
            //Debug.Log("겹치는 오브젝트 있음요");
            SetTransparent(0.3f, Color.red);
        }
        else
        {
            SetTransparent(0.3f, Color.green);
        }


    }

    //미리보기 아이템 삭제
    void DestoryPreviewPrefab()
    {
        if (previewBarricadPrefab != null)
        {
            Destroy(previewBarricadPrefab);
            Destroy(gameObject);
            previewBarricadPrefab = null;
        }

    }

    //바닥 포지션
    Vector3 GroundPos()
    {
        var tempPos = transform.position + transform.forward * installMaxDistance;

        //if (isGrab == true)
        //{
        //    isGrab = false;
        //    Vector3 pos1 = previewBarricadPrefab.transform.position;
        //    pos1.y += 90;

        //    previewBarricadPrefab.transform.Rotate(pos1);

        //}
        if(isGrab == true)
        {
            isGrab = false;
            rotationY += 90;
            if (rotationY >= 360f)
            {
                rotationY -= 360f;
            }
        }

        if (Physics.Raycast(tempPos, Vector3.down, out RaycastHit hit, 10f))
        {
            if (hit.collider.gameObject.tag == "Ground")
            {
                //포지션
                Vector3 spawnPos = hit.point;
                spawnPos.y += previewBounds.extents.y;
                var tempSpawnPos = spawnPos.y;

                spawnPos += transform.forward * installMaxDistance;
                previewBarricadPrefab.transform.position = spawnPos;

                //로테이션
                Vector3 direction = transform.position - previewBarricadPrefab.transform.position;
                direction.y = 0;

                Quaternion rotation = Quaternion.LookRotation(direction);

                rotation *= Quaternion.Euler(0, rotationY, 180);
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

    public bool Use(Transform firePos, int power)
    {
        if (CanPlace() == false)
        {
            PhotonNetwork.Instantiate("Barricade", GroundPos(), previewBarricadPrefab.transform.rotation);
            DestoryPreviewPrefab();
            return true;
        }
        else
        {
            //Debug.Log("겹쳐서 배치 불가!");
            return false;
        }

    }

   
}
