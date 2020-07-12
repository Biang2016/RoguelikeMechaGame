using GameCore;
using GameCore.AbilityDataDriven;
using Sirenix.OdinInspector;
using UnityEngine;
using Event = GameCore.AbilityDataDriven.Event;

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

        void LogicTick_Fighting()
        {
            if (ControlManager.Instance.CheckButtonAction_Instantaneously(TriggerButtonState))
            {
                TriggerAbilities();
            }

            if (ControlManager.Instance.CheckButtonAction_Continuously(TriggerButtonState))
            {
                ContinuousTriggerAbilities();
            }
        }

        public void TriggerAbilities()
        {
            foreach (Ability ability in MechaComponentInfo.AbilityGroup.Abilities)
            {
                if (!ability.Passive && ability.AbilityPowerCost < 100) // todo power
                {
                    foreach (Event evnt in ability.Events)
                    {
                        if (evnt.EventType == ENUM_Event.OnAbilityStart)
                        {
                            foreach (Action action in evnt.Actions)
                            {
                                if (action is Action_EmitProjectile_DelayLine act)
                                {
                                    switch (ability.CastDummyPosition)
                                    {
                                        case ENUM_AbilityCastDummyPosition.ShooterDummyPos:
                                        {
                                            ProjectileInfo pi = new ProjectileInfo(act, MechaType, act.ProjectileType);
                                            ProjectileManager.Instance.ShootProjectile(pi, ShooterDummyPos.position, ShooterDummyPos.forward);
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