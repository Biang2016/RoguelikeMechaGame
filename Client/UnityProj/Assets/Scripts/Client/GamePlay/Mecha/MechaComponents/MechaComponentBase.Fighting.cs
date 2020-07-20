using System.Collections.Generic;
using GameCore;
using GameCore.AbilityDataDriven;
using Google.Protobuf.WellKnownTypes;
using Sirenix.OdinInspector;
using UnityEngine;
using Enum = System.Enum;

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
            foreach (GamePlayAbility ability in MechaComponentInfo.AbilityGroup.Abilities)
            {
                ability.cooldownTicker = 0;
                ClientGameManager.Instance.BattleMessenger.AddListener<ExecuteInfo>((uint) ENUM_Event.OnAbilityStart, (executeInfo) =>
                {
                    if (ability == executeInfo.Ability)
                    {
                        ability.cooldownTicker = 0;
                    }
                });
                foreach (KeyValuePair<ENUM_Event, GamePlayEvent> kv in ability.EventDict)
                {
                    foreach (GamePlayAction action in kv.Value.Actions)
                    {
                        ClientGameManager.Instance.BattleMessenger.AddListener<ExecuteInfo>((uint) kv.Key, (executeInfo) =>
                        {
                            if (ability == executeInfo.Ability)
                            {
                                action.Execute(executeInfo);
                            }
                        });
                    }
                }
            }
        }

        private void Update_Fighting()
        {
            foreach (GamePlayAbility ability in MechaComponentInfo.AbilityGroup.Abilities)
            {
                if (ability.cooldownTicker <= ability.AbilityCooldown)
                {
                    ability.cooldownTicker += Mathf.RoundToInt(Time.deltaTime * 1000);
                }
            }

            if (ControlManager.Instance.CheckButtonAction(TriggerButtonState))
            {
                foreach (GamePlayAbility ability in MechaComponentInfo.AbilityGroup.Abilities)
                {
                    if (ability.canTriggered)
                    {
                        if (!ability.Passive && ability.AbilityPowerCost < 100) // todo power
                        {
                            ClientGameManager.Instance.BattleMessenger.Broadcast<ExecuteInfo>((uint) ENUM_Event.OnAbilityStart, new ExecuteInfo
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
    }
}