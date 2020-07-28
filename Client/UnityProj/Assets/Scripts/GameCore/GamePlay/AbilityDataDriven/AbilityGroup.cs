using System.Collections.Generic;
using BiangStudio.CloneVariant;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore.AbilityDataDriven
{
    public class AbilityGroup : IClone<AbilityGroup>
    {
        public string AbilityGroupName;

        [HideInInspector]
        public List<string> AbilityNames = new List<string>();

        [LabelText("技能列表")]
        [ListDrawerSettings(ListElementLabelName = "AbilityName")]
        public List<Ability> Abilities = new List<Ability>();

        public AbilityGroup Clone()
        {
            AbilityGroup ag = new AbilityGroup();
            ag.AbilityGroupName = AbilityGroupName;
            ag.AbilityNames = AbilityNames.Clone();
            ag.Abilities = Abilities.Clone();
            return ag;
        }
    }
}