//using Ilumisoft.RadarSystem.UI;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Ilumisoft.RadarSystem
//{
//    /// <summary>
//    /// 레이더 시스템의 핵심 클래스
//    /// - 지정된 오브젝트(Locatable)를 UI 상에서 아이콘으로 표시
//    /// - 플레이어를 기준으로 아이콘을 레이더 위에 배치
//    /// - 플레이어의 회전에 따라 회전 적용 가능
//    /// </summary>
//    [AddComponentMenu("Radar System/Radar")]
//    [DefaultExecutionOrder(-10)]
//    public class Radar : MonoBehaviour
//    {
//        /// <summary>
//        /// 각 Locatable 오브젝트와 해당 아이콘 UI 컴포넌트를 매핑하는 Dictionary
//        /// </summary>
//        readonly Dictionary<LocatableComponent, LocatableIconComponent> locatableIconDictionary = new();

//        [SerializeField]
//        [Tooltip("아이콘이 붙을 UI 컨테이너 (레이더 내부 영역)")]
//        private RectTransform iconContainer;

//        [SerializeField, Min(1)]
//        [Tooltip("레이더 감지 범위 (단위: 미터)")]
//        private float range = 20;

//        [SerializeField]
//        [Tooltip("플레이어 회전값을 레이더에 적용할지 여부")]
//        private bool applyRotation = true;

//        /// <summary>
//        /// 감지 범위 프로퍼티
//        /// </summary>
//        public float Range { get => range; set => range = value; }

//        /// <summary>
//        /// 회전 적용 여부 프로퍼티
//        /// </summary>
//        public bool ApplyRotation { get => applyRotation; set => applyRotation = value; }

//        /// <summary>
//        /// 플레이어 참조 (레이더 기준점)
//        /// </summary>
//        public GameObject Player;

//        private void OnEnable()
//        {
//            // Locatable 오브젝트가 추가/제거될 때 이벤트 연결
//            LocatableManager.OnLocatableAdded += OnLocatableAdded;
//            LocatableManager.OnLocatableRemoved += OnLocatableRemoved;
//        }

//        private void OnDisable()
//        {
//            // 이벤트 해제
//            LocatableManager.OnLocatableAdded -= OnLocatableAdded;
//            LocatableManager.OnLocatableRemoved -= OnLocatableRemoved;
//        }

//        /// <summary>
//        /// Locatable 오브젝트가 추가되었을 때 호출됨
//        /// - 아이콘을 생성하여 Dictionary에 저장
//        /// </summary>
//        private void OnLocatableAdded(LocatableComponent locatable)
//        {
//            if (locatable != null && !locatableIconDictionary.ContainsKey(locatable))
//            {
//                var icon = locatable.CreateIcon();
//                icon.transform.SetParent(iconContainer.transform, false);
//                locatableIconDictionary.Add(locatable, icon);
//            }
//        }

//        /// <summary>
//        /// Locatable 오브젝트가 제거되었을 때 호출됨
//        /// - 아이콘 제거 및 Dictionary에서 삭제
//        /// </summary>
//        private void OnLocatableRemoved(LocatableComponent locatable)
//        {
//            if (locatable != null && locatableIconDictionary.TryGetValue(locatable, out LocatableIconComponent icon))
//            {
//                locatableIconDictionary.Remove(locatable);
//                Destroy(icon.gameObject);
//            }
//        }

//        private void Update()
//        {
//            if (Player != null)
//            {
//                UpdateLocatableIcons();
//            }
//        }

//        /// <summary>
//        /// 모든 아이콘의 위치를 매 프레임마다 업데이트
//        /// </summary>
//        private void UpdateLocatableIcons()
//        {
//            foreach (var locatable in locatableIconDictionary.Keys)
//            {
//                if (locatableIconDictionary.TryGetValue(locatable, out var icon))
//                {
//                    if (TryGetIconLocation(locatable, out var iconLocation))
//                    {
//                        icon.SetVisible(true);
//                        icon.GetComponent<RectTransform>().anchoredPosition = iconLocation;
//                    }
//                    else
//                    {
//                        icon.SetVisible(false);
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// 아이콘이 레이더 UI 상 어디에 배치될지 계산
//        /// - 화면 내일 경우 위치 반환
//        /// - 화면 밖일 경우 false 반환
//        /// </summary>
//        private bool TryGetIconLocation(LocatableComponent locatable, out Vector2 iconLocation)
//        {
//            // 플레이어로부터 상대 위치 계산 (x, z 평면 기준)
//            iconLocation = GetDistanceToPlayer(locatable);

//            float radarSize = GetRadarUISize();
//            float scale = radarSize / Range;

//            iconLocation *= scale;

//            // 회전 적용 여부에 따라 방향 회전
//            if (ApplyRotation)
//            {
//                var forward = Vector3.ProjectOnPlane(Player.transform.forward, Vector3.up);
//                var rotation = Quaternion.LookRotation(forward);
//                var euler = rotation.eulerAngles;

//                // 좌우 반전 (UI 공간 기준 회전 방향 보정)
//                euler.y = -euler.y;
//                rotation.eulerAngles = euler;

//                var rotated = rotation * new Vector3(iconLocation.x, 0.0f, iconLocation.y);
//                iconLocation = new Vector2(rotated.x, rotated.z);
//            }

//            // 범위 내일 경우 Clamp 처리 후 위치 반환
//            if (iconLocation.sqrMagnitude < radarSize * radarSize || locatable.ClampOnRadar)
//            {
//                iconLocation = Vector2.ClampMagnitude(iconLocation, radarSize);
//                return true;
//            }

//            return false;
//        }

//        /// <summary>
//        /// 레이더 UI 반지름 구하기
//        /// </summary>
//        private float GetRadarUISize()
//        {
//            return iconContainer.rect.width / 2;
//        }

//        /// <summary>
//        /// 플레이어로부터 대상까지의 거리 (x, z 축 기준)
//        /// </summary>
//        private Vector2 GetDistanceToPlayer(LocatableComponent locatable)
//        {
//            Vector3 distance = locatable.transform.position - Player.transform.position;
//            return new Vector2(distance.x, distance.z);
//        }
//    }
//}

using Ilumisoft.RadarSystem.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Ilumisoft.RadarSystem
{
    /// <summary>
    /// Radar 시스템
    /// 
    /// 이 클래스는 지정된 Locatable 오브젝트들을 레이더 UI에 아이콘 형태로 표시하는 역할을 한다.
    /// - Player를 기준으로 거리 및 방향을 계산하여 아이콘 위치를 지정
    /// - 회전이 활성화된 경우, RotatingContainer의 회전을 통해 전체 아이콘들을 회전
    /// - iconContainer에 아이콘을 생성 및 관리
    /// </summary>
    [AddComponentMenu("Radar System/Radar")]
    [DefaultExecutionOrder(-10)]
    public class Radar : MonoBehaviour
    {
        /// <summary>
        /// 감지 대상 오브젝트와 해당 UI 아이콘을 매핑하는 사전
        /// </summary>
        private readonly Dictionary<LocatableComponent, LocatableIconComponent> locatableIconDictionary = new();

        [Header("UI 연결")]
        [SerializeField]
        [Tooltip("아이콘이 배치될 컨테이너 (일반적으로 RotatingContainer)")]
        private RectTransform iconContainer;

        [SerializeField]
        [Tooltip("회전이 적용될 오브젝트 (RotatingContainer)")]
        private Transform rotatingRoot;

        [Header("설정")]
        [SerializeField, Min(1)]
        [Tooltip("레이더 감지 거리 범위 (단위: 미터)")]
        private float range = 20;

        [SerializeField]
        [Tooltip("플레이어 회전을 레이더 회전에 반영할지 여부")]
        private bool applyRotation = true;

        [Tooltip("레이더의 기준이 되는 플레이어 오브젝트")]
        public GameObject Player;

        /// <summary>
        /// 감지 거리 설정
        /// </summary>
        public float Range { get => range; set => range = value; }

        /// <summary>
        /// 회전 반영 여부 설정
        /// </summary>
        public bool ApplyRotation { get => applyRotation; set => applyRotation = value; }

        private void OnEnable()
        {
            LocatableManager.OnLocatableAdded += OnLocatableAdded;
            LocatableManager.OnLocatableRemoved += OnLocatableRemoved;
        }

        private void OnDisable()
        {
            LocatableManager.OnLocatableAdded -= OnLocatableAdded;
            LocatableManager.OnLocatableRemoved -= OnLocatableRemoved;
        }

        /// <summary>
        /// 새로운 Locatable이 추가되었을 때 아이콘 생성 및 등록
        /// </summary>
        private void OnLocatableAdded(LocatableComponent locatable)
        {
            if (locatable != null && !locatableIconDictionary.ContainsKey(locatable))
            {
                var icon = locatable.CreateIcon();
                icon.transform.SetParent(iconContainer.transform, false);
                locatableIconDictionary.Add(locatable, icon);
            }
        }

        /// <summary>
        /// Locatable이 제거되었을 때 아이콘 삭제 및 등록 해제
        /// </summary>
        private void OnLocatableRemoved(LocatableComponent locatable)
        {
            if (locatable != null && locatableIconDictionary.TryGetValue(locatable, out LocatableIconComponent icon))
            {
                locatableIconDictionary.Remove(locatable);
                Destroy(icon.gameObject);
            }
        }

        /// <summary>
        /// 매 프레임마다 아이콘 위치 업데이트 및 회전 적용
        /// </summary>
        private void Update()
        {
            if (Player != null)
            {
                UpdateRotation();
                UpdateLocatableIcons();
            }
        }

        /// <summary>
        /// 플레이어 회전에 따라 rotatingRoot에 회전 적용
        /// </summary>
        private void UpdateRotation()
        {
            if (applyRotation && rotatingRoot != null)
            {
                Vector3 forward = Vector3.ProjectOnPlane(Player.transform.forward, Vector3.up);

                if (forward.sqrMagnitude > 0.001f)
                {
                    Quaternion rotation = Quaternion.LookRotation(forward);
                    float zRotation = -rotation.eulerAngles.y;
                    rotatingRoot.localRotation = Quaternion.Euler(0, 0, zRotation);
                }
            }
        }

        /// <summary>
        /// 각 아이콘의 위치를 계산하여 화면에 배치
        /// </summary>
        private void UpdateLocatableIcons()
        {
            foreach (var locatable in locatableIconDictionary.Keys)
            {
                if (locatableIconDictionary.TryGetValue(locatable, out var icon))
                {
                    if (TryGetIconLocation(locatable, out var iconLocation))
                    {
                        icon.SetVisible(true);
                        icon.GetComponent<RectTransform>().anchoredPosition = iconLocation;
                    }
                    else
                    {
                        icon.SetVisible(false);
                    }
                }
            }
        }

        /// <summary>
        /// 특정 Locatable의 UI 상 위치 계산
        /// - 회전은 회전 컨테이너에서 처리하므로 계산 시엔 반영하지 않음
        /// </summary>
        private bool TryGetIconLocation(LocatableComponent locatable, out Vector2 iconLocation)
        {
            iconLocation = GetDistanceToPlayer(locatable);

            float radarSize = GetRadarUISize();
            float scale = radarSize / range;

            iconLocation *= scale;

            // 감지 범위 안에 있는 경우만 위치 적용
            if (iconLocation.sqrMagnitude < radarSize * radarSize || locatable.ClampOnRadar)
            {
                iconLocation = Vector2.ClampMagnitude(iconLocation, radarSize);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 레이더 UI의 반지름(픽셀 단위) 반환
        /// </summary>
        private float GetRadarUISize()
        {
            return iconContainer.rect.width / 2;
        }

        /// <summary>
        /// 플레이어 기준 대상의 상대 거리 계산 (X-Z 평면)
        /// </summary>
        private Vector2 GetDistanceToPlayer(LocatableComponent locatable)
        {
            Vector3 distance = locatable.transform.position - Player.transform.position;
            return new Vector2(distance.x, distance.z);
        }
    }
}

