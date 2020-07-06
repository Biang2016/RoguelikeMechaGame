using System.Collections.Generic;
using BiangStudio.ObjectPool;
using GameCore;
using UnityEngine.Events;

namespace Client
{
    public partial class Mecha : PoolObject
    {
        public MechaInfo MechaInfo;

        public SortedDictionary<int, MechaComponentBase> MechaComponentDict = new SortedDictionary<int, MechaComponentBase>();

        public UnityAction<Mecha> OnRemoveMechaSuc;

        public override void PoolRecycle()
        {
            base.PoolRecycle();
            foreach (KeyValuePair<int, MechaComponentBase> kv in MechaComponentDict)
            {
                kv.Value.PoolRecycle();
            }

            MechaComponentDict.Clear();
            OnRemoveMechaSuc = null;
        }

        public void Initialize(MechaInfo mechaInfo)
        {
            Clean();

            MechaInfo = mechaInfo;
            MechaInfo.OnAddMechaComponentInfoSuc = (mci) => AddMechaComponent(mci);
            MechaInfo.OnRemoveMechaInfoSuc += (mi) => OnRemoveMechaSuc?.Invoke(this);
            MechaInfo.OnDropMechaComponent = (mci) =>
            {
                MechaComponentDropSprite mcds = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.MechaComponentDropSprite]
                    .AllocateGameObject<MechaComponentDropSprite>(ClientBattleManager.Instance.MechaComponentDropSpriteContainerRoot);
                mcds.Initialize(mci, MechaComponentDict[mci.GUID].transform.position);
            };

            MechaInfo.MechaEditorContainer = new MechaEditorContainer(
                DragAreaDefines.MechaEditorArea.ToString(),
                DragAreaDefines.MechaEditorArea,
                ConfigManager.GridSize,
                ConfigManager.EDIT_AREA_FULL_SIZE,
                ConfigManager.EDIT_AREA_FULL_SIZE,
                false,
                0,
                () => ControlManager.Instance.Building_RotateItem.Down);
            MechaInfo.MechaEditorContainer.OnRemoveItemSucAction = (item) => ((MechaComponentInfo) item.ItemContentInfo).RemoveMechaComponentInfo();
            MechaInfo.MechaEditorContainer.RefreshInventoryGrids();
            MechaInfo.MechaEditorContainer.RefreshConflictAndIsolation();

            foreach (KeyValuePair<int, MechaComponentInfo> kv in mechaInfo.MechaComponentInfos)
            {
                AddMechaComponent(kv.Value);
            }

            Initialize_Building(mechaInfo);
            Initialize_Fighting(mechaInfo);
        }

        public void Clean()
        {
        }

        void Update()
        {
            if (MechaInfo.MechaType == MechaType.Player)
            {
                Update_Building();
                Update_Fighting();
            }
        }

        void FixedUpdate()
        {
            if (MechaInfo.MechaType == MechaType.Player)
            {
                FixedUpdate_Fighting();
            }

            MechaInfo.UpdateLifeChange();
        }

        void LateUpdate()
        {
            if (MechaInfo.MechaType == MechaType.Player)
            {
                if (GameStateManager.Instance.GetState() == GameState.Fighting)
                {
                    LateUpdate_Fighting();
                }

                if (GameStateManager.Instance.GetState() == GameState.Building)
                {
                    LateUpdate_Building();
                }
            }
        }

        public void SetShown(bool shown)
        {
            foreach (KeyValuePair<int, MechaComponentBase> kv in MechaComponentDict)
            {
                kv.Value.SetShown(shown);
            }
        }

        private void Die()
        {
            if (MechaInfo.MechaType == MechaType.Enemy)
            {
                OnRemoveMechaSuc?.Invoke(this);
                PoolRecycle(0.5f);
            }
            else
            {
                // TODO Endgame
            }
        }
    }
}