using UnityEngine;

namespace Ilumisoft.RadarSystem
{
    /// <summary>
    /// 레이더에 표시되는 아이콘 (몬스터/채집물/플레이어 등)
    /// </summary>
    public class RadarIcon : MonoBehaviour
    {
        [SerializeField]
        private GameObject graphic; // 아이콘 그래픽 오브젝트 (숨김 처리용)

        /// <summary>
        /// 아이콘을 켜거나 끈다
        /// </summary>
        /// <param name="visible">보이게 할지 여부
        public void SetVisible(bool visible)
        {
            if (graphic != null)
                graphic.SetActive(visible);
            else
                gameObject.SetActive(visible); // 그래픽이 없으면 전체 on/off
        }
    }
}
