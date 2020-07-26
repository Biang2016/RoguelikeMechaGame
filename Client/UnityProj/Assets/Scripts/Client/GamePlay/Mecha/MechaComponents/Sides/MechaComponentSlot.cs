using BiangStudio.GameDataFormat.Grid;
using GameCore;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Client
{
    [ExecuteInEditMode]
    public class MechaComponentSlot : MonoBehaviour
    {
        [SerializeField]
        private MeshRenderer SlotLightRenderer;

        [SerializeField]
        private Material[] SlotLightMaterials;

        private bool inUse;

        /// <summary>
        /// 游戏中实际生效的槽位
        /// </summary>
        public bool InUse
        {
            get { return inUse; }
            set
            {
                inUse = value;
                gameObject.SetActive(value);
            }
        }

        private bool isCandidate;

        /// <summary>
        /// 可作为概率随机的候选槽位
        /// </summary>
        public bool IsCandidate
        {
            get { return isCandidate; }
            set
            {
                isCandidate = value;
                if (!Application.isPlaying)
                {
                    gameObject.SetActive(value);
#if UNITY_EDITOR
                    SceneView.RepaintAll();
#endif
                }
            }
        }

        [SerializeField]
        [OnValueChanged("OnChangeSlotType")]
        public SlotType SlotType;

        internal GridPosR.Orientation Orientation;

        public void OnChangeSlotType()
        {
            SlotLightRenderer.material = SlotLightMaterials[(int) SlotType];
        }

        public void Initialize()
        {
            Orientation = GridPosR.GetGridPosByLocalTrans(transform, ConfigManager.GridSize).orientation;
        }

        void Update()
        {
        }

        public void SetShown(bool shown)
        {
            SlotLightRenderer.enabled = shown;
        }
    }

    public enum SlotType
    {
        Yellow = 0,
        Red = 1,
        Blue = 2,
        Green = 3,
    }
}