using System.Collections.Generic;
using BiangStudio.CloneVariant;
using Sirenix.OdinInspector;

namespace GameCore.AbilityDataDriven
{
    public class GamePlayEvent : IClone<GamePlayEvent>
    {
        public string EventName => EventType.ToString();

        [LabelText("触发时机")]
        public ENUM_Event EventType;

        [LabelText("行为列表")]
        [ListDrawerSettings(ListElementLabelName = "ActionName")]
        public List<GamePlayAction> Actions = new List<GamePlayAction>();

        public GamePlayEvent Clone()
        {
            return new GamePlayEvent
            {
                Actions = Actions.Clone(),
                EventType = EventType,
            };
        }
    }
}