using System;
using System.Collections.Generic;
using BiangStudio.CloneVariant;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.ShapedInventory;
using UnityEngine.Events;

namespace GameCore
{
    [Serializable]
    public class MechaComponentInfo : IInventoryItemContentInfo
    {
        public int GUID;
        private static int guidGenerator = (int)ConfigManager.GUID_Separator.MechaComponentInfo;

        private int GetGUID()
        {
            return guidGenerator++;
        }

        public MechaComponentType MechaComponentType;
        public GridPosR GridPos;
        public List<GridPos> OccupiedGridPositions = new List<GridPos>();
        public List<GridPos> OriginalOccupiedGridPositions => OccupiedGridPositions;

        public UnityAction<MechaComponentInfo> OnRemoveMechaComponentInfoSuc;

        public string ItemSpriteKey => typeof(MechaComponentType).FullName + "." + MechaComponentType;

        public string ItemName => "机甲组件." + MechaComponentType;

        public MechaComponentInfo(MechaComponentType mechaComponentType, GridPosR gridPos, int totalLife, int dropProbability)
        {
            GUID = GetGUID();
            MechaComponentType = mechaComponentType;
            GridPos = gridPos;
            TotalLife = totalLife;
            M_LeftLife = totalLife;
            DropProbability = dropProbability;
            if (ConfigManager.MechaComponentOccupiedGridPosDict.TryGetValue(mechaComponentType, out List<GridPos> ops))
            {
                OccupiedGridPositions = ops.Clone();
            }
        }

        public MechaComponentInfo Clone()
        {
            MechaComponentInfo mci = new MechaComponentInfo(MechaComponentType, GridPos, TotalLife, DropProbability);
            mci.OccupiedGridPositions = OccupiedGridPositions.Clone();
            return mci;
        }

        public void RemoveMechaComponentInfo()
        {
            OnRemoveMechaComponentInfoSuc?.Invoke(this);
        }

        #region Life

        public int DropProbability;
        public int TotalLife;

        internal bool IsDead = false;

        public UnityAction<int, int> OnLifeChange;

        private int _leftLife;

        public int M_LeftLife
        {
            get { return _leftLife; }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }

                if (_leftLife != value)
                {
                    _leftLife = value;
                    OnLifeChange?.Invoke(_leftLife, M_TotalLife);
                }
            }
        }

        private int _totalLife;

        public int M_TotalLife
        {
            get { return _totalLife; }
            set
            {
                if (_totalLife != value)
                {
                    _totalLife = value;
                    OnLifeChange?.Invoke(M_LeftLife, _totalLife);
                }
            }
        }

        public bool CheckAlive()
        {
            return M_LeftLife > 0;
        }

        public UnityAction<int> OnDamaged;

        public void Damage(int damage)
        {
            M_LeftLife -= damage;
            OnDamaged?.Invoke(damage);

            if (!IsDead && !CheckAlive())
            {
                IsDead = true;
                Died();
                OnDied?.Invoke();
            }
        }

        public UnityAction OnDied;

        private void Died()
        {
            OnRemoveMechaComponentInfoSuc?.Invoke(this);
        }

        #endregion
    }
}