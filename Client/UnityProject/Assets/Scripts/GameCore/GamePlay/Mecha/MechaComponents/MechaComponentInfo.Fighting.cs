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
                    logIdentityName = $"{MechaInfo.LogIdentityName}-<color=\"#7D67FF\">机甲组件.{ItemSpriteKey}</color>-{GUID}";
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
                    OnHighLightColorChange?.Invoke(QualityManager.GetQuality(Quality).Color, CurrentPowerUpgradeData.HighLightColorIntensity);
                }
            }
        }

        #endregion

        public bool TriggerButtonThisFrame = false;

        public void PreUpdate_Fighting()
        {
        }

        public void PowerUpdate_Fighting()
        {
            foreach (Ability ability in AbilityGroup.Abilities)
            {
                if (ability.Passive)
                {
                    BattleManager.Instance.BattleMessenger.Broadcast<ExecuteInfo>((uint) ENUM_AbilityEvent.OnPowerCalculate, new ExecuteInfo
                    {
                        MechaInfo = MechaInfo,
                        MechaComponentInfo = this,
                        Ability = ability
                    });
                }
            }
        }

        public void Update_Fighting()
        {
            float abilityCooldownFactor = 1f;
            switch (CurrentPowerUpgradeData)
            {
                case PowerUpgradeData_Gun pud_Gun:
                {
                    abilityCooldownFactor -= (pud_Gun.AbilityCooldownDecreasePercent / 100f);
                    break;
                }
            }

            foreach (Ability ability in AbilityGroup.Abilities)
            {
                if (ability.cooldownTicker < ability.AbilityCooldown)
                {
                    ability.cooldownTicker += Mathf.RoundToInt(Time.deltaTime * 1000 * (1f / abilityCooldownFactor));
                }
                else
                {
                    if (!ability.Passive && ability.AbilityBehaviors.HasFlag(ENUM_AbilityBehavior.ABILITY_BEHAVIOR_AUTOCAST))
                    {
                        BattleManager.Instance.BattleMessenger.Broadcast<ExecuteInfo>((uint) ENUM_AbilityEvent.OnAbilityStart, new ExecuteInfo
                        {
                            MechaInfo = MechaInfo,
                            MechaComponentInfo = this,
                            Ability = ability
                        });
                    }
                }
            }

            if (TriggerButtonThisFrame)
            {
                foreach (Ability ability in AbilityGroup.Abilities)
                {
                    if (ability.canTriggered && !ability.Passive)
                    {
                        if (!ability.Passive && !ability.AbilityBehaviors.HasFlag(ENUM_AbilityBehavior.ABILITY_BEHAVIOR_AUTOCAST))
                        {
                            if (ability.AbilityBehaviors.HasFlag(ENUM_AbilityBehavior.ABILITY_BEHAVIOR_FORBID_MOVEMENT))
                            {
                                MechaInfo.AbilityForbidMovement = true;
                            }

                            BattleManager.Instance.BattleMessenger.Broadcast<ExecuteInfo>((uint) ENUM_AbilityEvent.OnAbilityStart, new ExecuteInfo
                            {
                                MechaInfo = MechaInfo,
                                MechaComponentInfo = this,
                                Ability = ability
                            });
                        }
                    }
                }
            }
        }

        public void LateUpdate_Fighting()
        {
        }

        public void FixedUpdate_Fighting()
        {
        }
    }
}