using System;
using System.Collections.Generic;
using System.IO;
using BiangStudio.DragHover;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.GamePlay;
using BiangStudio.GridBackpack;
using BiangStudio.ShapedInventory;
using GameCore;
using GameCore.AbilityDataDriven;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;

#endif

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
            foreach (Ability ability in MechaComponentInfo.Abilities)
            {
            }

            FireByFirePointDirection();
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

        private void FireByFirePointDirection()
        {
            //ProjectileManager.Instance.ShootProjectile(ShooterInfo.ProjectileInfo, ShooterDummyPos.position, ShooterDummyPos.forward);
        }
    }
}