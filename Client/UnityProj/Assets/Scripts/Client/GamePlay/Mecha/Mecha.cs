using UnityEngine;
using System.Collections.Generic;
using BiangStudio.ObjectPool;
using GameCore;
using UnityEngine.Events;

namespace Client
{
    public partial class Mecha : PoolObject
    {
        public MechaInfo MechaInfo;

        private List<MechaComponentBase> mechaComponents = new List<MechaComponentBase>();

        private MechaComponentBase[,] mechaComponentMatrix = new MechaComponentBase[ConfigManager.EDIT_AREA_FULL_SIZE, ConfigManager.EDIT_AREA_FULL_SIZE]; //[z,x]

        public override void PoolRecycle()
        {
            base.PoolRecycle();
            foreach (MechaComponentBase mcb in mechaComponents)
            {
                mcb.PoolRecycle();
            }

            mechaComponents.Clear();
        }

        public void Initialize(MechaInfo mechaInfo)
        {
            mechaComponentMatrix = new MechaComponentBase[ConfigManager.EDIT_AREA_FULL_SIZE, ConfigManager.EDIT_AREA_FULL_SIZE];
            MechaInfo = mechaInfo;
            RefreshMechaMatrix();
            foreach (MechaComponentInfo mci in mechaInfo.MechaComponentInfos)
            {
                AddMechaComponent(mci);
            }

            Initialize_Building(mechaInfo);
            Initialize_Fighting(mechaInfo);

            M_TotalLife = 0;
            M_LeftLife = 1;
        }

        void Update()
        {
            if (MechaInfo.MechaType == MechaType.Self)
            {
                Update_Building();
                Update_Fighting();
            }
        }

        void FixedUpdate()
        {
            if (MechaInfo.MechaType == MechaType.Self)
            {
                FixedUpdate_Fighting();
            }

            UpdateLifeChange();
        }

        void LateUpdate()
        {
            if (MechaInfo.MechaType == MechaType.Self)
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
            foreach (MechaComponentBase mcb in mechaComponents)
            {
                mcb.SetShown(shown);
            }
        }

        private void Die()
        {
            if (MechaInfo.MechaType == MechaType.Enemy)
            {
                BattleManager.Instance.EnemyMechas.Remove(this);
                PoolRecycle(0.5f);
            }
            else
            {
                // TODO Endgame
            }
        }

        #region Life & Power

        private void UpdateLifeChange()
        {
            int totalLife = 0;
            int leftLife = 0;
            foreach (MechaComponentBase mcb in mechaComponents)
            {
                totalLife += mcb.M_TotalLife;
                leftLife += mcb.M_LeftLife;
            }

            M_TotalLife = Mathf.Max(M_TotalLife, totalLife);
            M_LeftLife = leftLife;
        }

        public UnityAction RefreshHUDPanelCoreLifeSliderCount;

        public List<MechaComponentBase> GetCoreLifeChangeDelegates()
        {
            List<MechaComponentBase> res = new List<MechaComponentBase>();
            foreach (MechaComponentBase mcb in mechaComponents)
            {
                if (mcb.MechaComponentInfo.MechaComponentType == MechaComponentType.Core)
                {
                    res.Add(mcb);
                }
            }

            return res;
        }

        internal UnityAction<int, int> OnLifeChange;

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

        internal UnityAction<int, int> OnPowerChange;

        private int _leftPower;

        public int M_LeftPower
        {
            get { return _leftPower; }
            set
            {
                if (_leftPower != value)
                {
                    _leftPower = value;
                    OnPowerChange?.Invoke(_leftPower, M_TotalPower);
                }
            }
        }

        private int _totalPower;

        public int M_TotalPower
        {
            get { return _totalPower; }
            set
            {
                if (_totalPower != value)
                {
                    _totalPower = value;
                    OnPowerChange?.Invoke(M_LeftPower, _totalPower);
                }
            }
        }

        #endregion
    }
}