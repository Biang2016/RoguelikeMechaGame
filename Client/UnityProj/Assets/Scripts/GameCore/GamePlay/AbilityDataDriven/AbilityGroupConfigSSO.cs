using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore.AbilityDataDriven
{
    [CreateAssetMenu(menuName = "BattleConfig/AbilityGroup")]
    public class AbilityGroupConfigSSO : SerializedScriptableObject
    {
        public string AbilityGroupName;

        [ListDrawerSettings(ListElementLabelName = "AbilityName")]
        public List<AbilityConfigSSO> AbilityConfigs = new List<AbilityConfigSSO>();

        public AbilityGroup GetAbilityGroup()
        {
            AbilityGroup ag = new AbilityGroup();
            ag.AbilityGroupName = AbilityGroupName;
            foreach (AbilityConfigSSO acsso in AbilityConfigs)
            {
                ag.AbilityNames.Add(acsso.Ability.AbilityName);
            }

            return ag;
        }
    }
}