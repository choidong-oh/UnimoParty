using UnityEngine;
using Ilumisoft.RadarSystem; // RadarIcon.cs가 정의된 네임스페이스
/// <summary>
/// 레이더에 표시될 수 있는 대상 오브젝트(몬스터, 플레이어, 채집물 등)에 붙는 스크립트
/// 이 스크립트를 통해 해당 오브젝트가 레이더 도트를 생성할 수 있게 된다.
/// </summary>
public class RadarTarget : MonoBehaviour
{
    /// <summary>
    /// 레이더 아이콘을 생성하는 함수
    /// 이 함수는 Radar에서 호출
    /// 오브젝트의 태그에 따라 해당하는 도트 프리팹을 불러와서 생성
    /// </summary>
    public RadarIcon CreateIcon()
    {
        GameObject iconPrefab = null;

        // 오브젝트의 태그에 따라 사용할 도트 프리팹 결정
        if (CompareTag("Monster"))
            iconPrefab = Resources.Load<GameObject>("RadarDot_Monster"); // 빨강: 몬스터
        else if (CompareTag("OtherPlayer"))
            iconPrefab = Resources.Load<GameObject>("RadarDot_OtherUser"); // 흰색: 다른플레아어
        else if (CompareTag("Object"))
            iconPrefab = Resources.Load<GameObject>("RadarDot_Object"); // 노랑: 채집물

        // 해당 태그에 맞는 프리팹을 찾지 못한 경우 경고 출력 후 null 반환
        if (iconPrefab == null)
        {
            Debug.LogWarning($"[Radar] 태그 '{gameObject.tag}'아이콘 프리팹이 없음");
            return null;
        }

        // 도트 프리팹 인스턴스 생성
        GameObject iconGO = Instantiate(iconPrefab);

        // 생성된 오브젝트에서 RadarIcon 컴포넌트를 반환
        return iconGO.GetComponent<RadarIcon>();
    }
}
