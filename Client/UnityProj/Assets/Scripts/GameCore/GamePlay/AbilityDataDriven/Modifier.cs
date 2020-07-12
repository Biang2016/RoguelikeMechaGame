using System.Collections.Generic;
using BiangStudio.CloneVariant;
using Sirenix.OdinInspector;

namespace GameCore.AbilityDataDriven
{
    public class Modifier : IClone<Modifier>
    {
        [LabelText("Modifier����")]
        public string ModifierName;

        [LabelText("����")]
        public ENUM_ModifierAttribute Attributes;

        [LabelText("����ʱ��")]
        [SuffixLabel("ms", true)]
        public int Duration;

        public bool IsBuff;
        public bool IsDeBuff;

        [LabelText("����")]
        public bool IsHidden;

        [LabelText("��������")]
        public bool Passive;

        [LabelText("�������")]
        [SuffixLabel("ms", true)]
        public int ThinkInterval;

        //public Dictionary<ENUM_ModifierProperty, Fix64> Properties = new Dictionary<ENUM_ModifierProperty, Fix64>();

        //public HashSet<ENUM_ModifierStates> States = new HashSet<ENUM_ModifierStates>();

        [LabelText("�����¼��б�")]
        [ListDrawerSettings(ListElementLabelName = "EventName")]
        public List<Event> Events = new List<Event>();

        public Modifier Clone()
        {
            return new Modifier
            {
                ModifierName = ModifierName,
                Attributes = Attributes,
                Duration = Duration,
                IsBuff = IsBuff,
                IsDeBuff = IsDeBuff,
                IsHidden = IsHidden,
                Passive = Passive,
                ThinkInterval = ThinkInterval,
                Events = Events.Clone(),
            };
        }
    }
}