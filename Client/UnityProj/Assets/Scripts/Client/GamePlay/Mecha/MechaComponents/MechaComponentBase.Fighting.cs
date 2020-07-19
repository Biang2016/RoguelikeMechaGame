using GameCore;
using GameCore.AbilityDataDriven;
using Google.Protobuf.WellKnownTypes;
using Sirenix.OdinInspector;
using UnityEngine;
using Enum = System.Enum;

namespace Client
{
    public abstract partial class MechaComponentBase
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

        void Update_Fighting()
        {
            if (ControlManager.Instance.CheckButtonAction_Instantaneously(TriggerButtonState))
            {
                TriggerAbilities();
            }

            if (ControlManager.Instance.Battle_Skill_2.Down)
            {
                projectileType = (ProjectileType) (((int) projectileType + 1) % Enum.GetValues(typeof(ProjectileType)).Length);
                Debug.Log(projectileType);
            }
        }

        private ProjectileType projectileType;

        public void TriggerAbilities()
        {
            foreach (GamePlayAbility ability in MechaComponentInfo.AbilityGroup.Abilities)
            {
                if (!ability.Passive && ability.AbilityPowerCost < 100) // todo power
                {
                    foreach (GamePlayEvent evnt in ability.Events)
                    {
                        if (evnt.EventType == ENUM_Event.OnAbilityStart)
                        {
                            foreach (GamePlayAction action in evnt.Actions)
                            {
                                if (action is Action_EmitProjectile act)
                                {
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

        public void ContinuousTriggerAbilities()
        {
            //if (fireCountdown <= 0f)
            //{
            //    FireByFirePointDirection();
            //    fireCountdown = 0;
            //    fireCountdown += ShooterInfo.FireInterval;
            //}
        }
    }
}