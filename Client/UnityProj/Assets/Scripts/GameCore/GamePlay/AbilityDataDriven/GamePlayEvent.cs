using System.Collections.Generic;
using BiangStudio.CloneVariant;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore.AbilityDataDriven
{
    public class GamePlayEvent : IClone<GamePlayEvent>
    {
        [HideInInspector]
        public string EventName => EventType.ToString();

        [LabelText("时机")]
        public ENUM_Event EventType;

        [LabelText("行为列表")]
        [ListDrawerSettings(ListElementLabelName = "ActionName")]
        public List<GamePlayAction> Actions = new List<GamePlayAction>();

        public GamePlayEvent Clone()
        {
            return new GamePlayEvent
            {
                EventType = EventType,
                Actions = Actions.Clone(),
            };
        }
    }
}