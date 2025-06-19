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
        [Tooltip("아이콘이 배치될 컨테이너 (RotatingContainer)")]
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
        private float radarUISize;

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
        /// 외부에서 강제로 레이더 회전을 갱신할 때 호출
        /// </summary>
        public void RefreshRotationImmediately()
        {
            UpdateRotation();
        }
        ///
        ///
        ///


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

            //  너무 가까우면 최소 반지름 강제 적용 (중심 과밀 방지)
            float minRadius = radarUISize * 0.25f; // 최소 거리 UI 단위 (비율로 조절 가능)

            if (iconLocation.magnitude < minRadius)
            {
                iconLocation = iconLocation.normalized * minRadius;


            }



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

