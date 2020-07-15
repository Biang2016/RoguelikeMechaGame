using UnityEngine;
using System.Collections;
using Client;
using GameCore;
using NodeCanvas.BehaviourTrees;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions
{
    [Name("等待CD")]
    public class WaitAction : ActionTask
    {
        public BBParameter<float> CD;

        protected override void OnUpdate()
        {
            if (elapsedTime > CD.value)
            {
                EndAction(true);
            }
        }
    }

    [Name("点按技能按键")]
    public class AbilityButtonTriggerOnce : ConditionTask
    {
        public BBParameter<ButtonState> ButtonState;

        protected override bool OnCheck()
        {
            return ControlManager.Instance.CheckButtonAction_Instantaneously(ButtonState.value);
        }
    }

    [Name("长按技能按键")]
    public class AbilityButtonTriggerContinuously : ConditionTask
    {
        public BBParameter<ButtonState> ButtonState;

        protected override bool OnCheck()
        {
            return ControlManager.Instance.CheckButtonAction_Continuously(ButtonState.value);
        }
    }
}