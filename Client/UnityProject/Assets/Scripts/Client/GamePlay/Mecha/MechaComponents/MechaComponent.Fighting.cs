using System.Collections.Generic;
using GameCore;
using GameCore.AbilityDataDriven;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Client
{
    public partial class MechaComponent
    {
        [LabelText("触发按键")]
        [PropertyOrder(9)]
        [TitleGroup("Inputs")]
        public ButtonState TriggerButtonState;

        [SerializeField]
        [PropertyOrder(8)]
        [TitleGroup("DummyPositions")]
        [LabelText("射击锚点")]
        private Transform ShooterDummyPos;

        [SerializeField]
        [PropertyOrder(8)]
        [TitleGroup("DummyPositions")]
        [LabelText("其他锚点（示例）")]
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

        public void PreUpdate_Fighting()
        {
            MechaComponentInfo.AccumulatedPowerInsideThisFrame = 0;
            MechaComponentInfo.TriggerButtonThisFrame = ControlManager.Instance.CheckButtonAction(TriggerButtonState);
        }

        private void OnDamaged(MechaComponentInfo attacker, int damage)
        {
            ClientGameManager.Instance.BattleMessenger.Broadcast<AttackData>((uint) ENUM_BattleEvent.Battle_MechaComponentAttackTip,
                new AttackData(attacker, this, damage, BattleTipType.Attack, 0, 0));
            MechaComponentModelRoot.OnDamage((float) MechaComponentInfo.M_LeftLife / MechaComponentInfo.M_TotalLife);
            MechaComponentModelRoot.SetAnimTrigger("OnHit");
        }
    }
}