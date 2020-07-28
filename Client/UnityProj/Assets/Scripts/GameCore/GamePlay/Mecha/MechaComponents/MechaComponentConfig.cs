using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore
{
    public class MechaComponentConfig
    {
        [LabelText("品质")]
        public Quality Quality;

        [LabelText("生命值")]
        public int Life;

        [LabelText("消耗功率")]
        public int PowerConsume;
    }
}