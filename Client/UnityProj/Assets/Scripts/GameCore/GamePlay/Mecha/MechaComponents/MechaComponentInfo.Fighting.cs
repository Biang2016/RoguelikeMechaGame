using GameCore.AbilityDataDriven;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace GameCore
{
    public partial class MechaComponentInfo
    {
        #region GamePlay Info

        public MechaComponentConfig MechaComponentConfig;

        [LabelText("品质")]
        [HideInEditorMode]
        public Quality Quality;

        private MechaInfo mechaInfo;

        public MechaInfo MechaInfo
        {
            get { return mechaInfo; }
            set
            {
                mechaInfo = value;
                if (mechaInfo != null)
                {
                    logIdentityName = $"{MechaInfo.LogIdentityName}-<color=\"#7D67FF\">{ItemName}</color>-{GUID}";
                }
            }
        }

        [ReadOnly]
        [ShowInInspector]
        [HideInEditorMode]
        [LabelText("技能组")]
        public AbilityGroup AbilityGroup;

        [ReadOnly]
        [ShowInInspector]
        [HideInEditorMode]
        [LabelText("品质配置")]
        public MechaComponentQualityConfig MechaComponentQualityConfig;

        public QualityUpgradeDataBase CurrentQualityUpgradeData;
        public PowerUpgradeDataBase CurrentPowerUpgradeData;

        #endregion

        #region Life

        [HideInInspector]
        public bool IsDead = false;

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

        public UnityAction<MechaComponentInfo, int> OnDamaged;

        public void Damage(MechaComponentInfo attacker, int damage)
        {
            M_LeftLife -= damage;
            OnDamaged?.Invoke(attacker, damage);

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

        #region Power

        public UnityAction<int> OnInputPowerChange;

        public int AccumulatedPowerInsideThisFrame;

        private int _inputPower;

        public int M_InputPower
        {
            get { return _inputPower; }
            set
            {
                if (_inputPower != value)
                {
                    _inputPower = value;
                    OnInputPowerChange?.Invoke(_inputPower);
                    CurrentPowerUpgradeData = CurrentQualityUpgradeData.GetPowerUpgradeData(_inputPower);
                    OnHighLightColorChange?.Invoke(CurrentQualityUpgradeData.HighLightColor, CurrentPowerUpgradeData.HighLightColorIntensity);
                }
            }
        }

        #endregion
    }
}