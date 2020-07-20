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

        private void Initialize_Fighting()
        {
            foreach (GamePlayAbility ability in MechaComponentInfo.AbilityGroup.Abilities)
            {
                ability.cooldownTicker = 0;
                foreach (KeyValuePair<ENUM_Event, GamePlayEvent> kv in ability.Events)
                {
                    switch (kv.Key)
                    {
                        case ENUM_Event.OnAbilityStart:
                        {
                            foreach (GamePlayAction action in kv.Value.Actions)
                            {
                                ClientGameManager.Instance.BattleMessenger.AddListener<ExecuteInfo>((uint) ENUM_Event.OnAbilityStart, (executeInfo) => { action.Execute(executeInfo); });
                            }

                            break;
                        }
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
                                MechaGUID = MechaInfo.GUID, MechaComponentGUID = MechaComponentInfo.GUID, AbilityName = ability.AbilityName
                            });

                            foreach (KeyValuePair<ENUM_Event, GamePlayEvent> kv in ability.Events)
                            {
                                if (kv.Key == ENUM_Event.OnAbilityStart)
                                {
                                    ability.cooldownTicker = 0;
                                    foreach (GamePlayAction action in kv.Value.Actions)
                                    {
                                        if (action is Action_EmitProjectile act)
                                        {
                                            act.OnHit += flyRealTimeData => { };
                                            switch (ability.CastDummyPosition)
                                            {
                                                case ENUM_AbilityCastDummyPosition.ShooterDummyPos:
                                                {
                                                    ProjectileInfo pi = new ProjectileInfo(act, MechaComponentInfo, MechaInfo, null, Vector3.zero);
                                                    //pi.ParentAction.ProjectileConfig.ProjectileType = projectileType;
                                                    ClientProjectileManager.Instance.ShootProjectile(pi, ShooterDummyPos.position, ShooterDummyPos.forward);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (ControlManager.Instance.Battle_Skill_2.Down)
            {
                projectileType = (ProjectileType) (((int) projectileType + 1) % Enum.GetValues(typeof(ProjectileType)).Length);
                Debug.Log(projectileType);
            }
        }

        private ProjectileType projectileType;
    }
}