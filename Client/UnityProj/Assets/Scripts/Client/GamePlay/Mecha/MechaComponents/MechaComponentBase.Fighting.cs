using System.Collections.Generic;
using GameCore;
using GameCore.AbilityDataDriven;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Client
{
    public partial class MechaComponentBase
    {
        [LabelText("触发按键")]
        [PropertyOrder(9)]
        [TitleGroup("Inputs")]
        public ButtonState TriggerButtonState;

        [SerializeField]
        [PropertyOrder(-8)]
        [TitleGroup("DummyPositions")]
        private Transform ShooterDummyPos;

        [SerializeField]
        [PropertyOrder(-8)]
        [TitleGroup("DummyPositions")]
        private Transform AnotherSampleDummyPos;

        public Dictionary<ENUM_ProjectileDummyPosition, Transform> DummyPosDict = new Dictionary<ENUM_ProjectileDummyPosition, Transform>();

        private void Awake_Fighting()
        {
            DummyPosDict.Add(ENUM_ProjectileDummyPosition.ShooterDummyPos, ShooterDummyPos);
        }

        private void Initialize_Fighting()
        {
            MechaComponentInfo.OnDamaged += OnDamaged;
            RegisterAbilityEvents();
        }

        private void RegisterAbilityEvents()
        {
            foreach (Ability ability in MechaComponentInfo.AbilityGroup.Abilities)
            {
                ability.cooldownTicker = 0;
                ClientGameManager.Instance.BattleMessenger.AddListener<ExecuteInfo>((uint) ENUM_AbilityEvent.OnAbilityStart, (executeInfo) =>
                {
                    if (ability == executeInfo.Ability)
                    {
                        ability.cooldownTicker = 0;
                    }
                });
                foreach (KeyValuePair<ENUM_AbilityEvent, GamePlayEvent> kv in ability.EventDict)
                {
                    foreach (GamePlayAction action in kv.Value.Actions)
                    {
                        action.OnRegisterEvent(ClientGameManager.Instance.BattleMessenger, kv.Key, ability);
                    }
                }
            }
        }

        private void UnregisterAbilityEvents()
        {
            // todo
        }

        public void Update_Fighting()
        {
            float abilityCooldownFactor = 1f;
            switch (MechaComponentInfo.CurrentPowerUpgradeData)
            {
                case PowerUpgradeData_Gun pud_Gun:
                {
                    abilityCooldownFactor -= (pud_Gun.AbilityCooldownDecreasePercent / 100f);
                    break;
                }
            }

            foreach (Ability ability in MechaComponentInfo.AbilityGroup.Abilities)
            {
                if (ability.cooldownTicker <= ability.AbilityCooldown)
                {
                    ability.cooldownTicker += Mathf.RoundToInt(Time.deltaTime * 1000 * (1f / abilityCooldownFactor));
                }
                else
                {
                    if (!ability.Passive && ability.AbilityBehaviors.HasFlag(ENUM_AbilityBehavior.ABILITY_BEHAVIOR_AUTOCAST))
                    {
                        ClientGameManager.Instance.BattleMessenger.Broadcast<ExecuteInfo>((uint) ENUM_AbilityEvent.OnAbilityStart, new ExecuteInfo
                        {
                            MechaInfo = MechaInfo,
                            MechaComponentInfo = MechaComponentInfo,
                            Ability = ability
                        });
                    }
                }
            }

            if (ControlManager.Instance.CheckButtonAction(TriggerButtonState))
            {
                foreach (Ability ability in MechaComponentInfo.AbilityGroup.Abilities)
                {
                    if (ability.canTriggered && !ability.Passive)
                    {
                        if (!ability.Passive && !ability.AbilityBehaviors.HasFlag(ENUM_AbilityBehavior.ABILITY_BEHAVIOR_AUTOCAST))
                        {
                            if (ability.AbilityBehaviors.HasFlag(ENUM_AbilityBehavior.ABILITY_BEHAVIOR_FORBID_MOVEMENT))
                            {
                                Mecha.AbilityForbidMovement = true;
                            }

                            ClientGameManager.Instance.BattleMessenger.Broadcast<ExecuteInfo>((uint) ENUM_AbilityEvent.OnAbilityStart, new ExecuteInfo
                            {
                                MechaInfo = MechaInfo,
                                MechaComponentInfo = MechaComponentInfo,
                                Ability = ability
                            });
                        }
                    }
                }
            }
        }

        private void OnDamaged(MechaComponentInfo attacker, int damage)
        {
            ClientGameManager.Instance.BattleMessenger.Broadcast<AttackData>((uint) ENUM_BattleEvent.Battle_MechaComponentAttackTip,
                new AttackData(attacker, this, damage, BattleTipType.Attack, 0, 0));
            MechaComponentModelRoot.OnDamage((float) MechaComponentInfo.M_LeftLife / MechaComponentInfo.M_TotalLife);
        }
    }
}