﻿using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore.AbilityDataDriven
{
    [CreateAssetMenu(menuName = "BattleConfig/AbilityGroup")]
    public class AbilityGroupConfigSSO : SerializedScriptableObject
    {
        public string AbilityGroupName => name;

        [LabelText("技能配置列表（工具）")]
        [ListDrawerSettings(ListElementLabelName = "AbilityName")]
        public List<AbilityConfigSSO> AbilityConfigs = new List<AbilityConfigSSO>();

        [ReadOnly]
        [ShowInInspector]
        public AbilityGroup AbilityGroup
        {
            get
            {
                AbilityGroup ag = new AbilityGroup();
                ag.AbilityGroupName = AbilityGroupName;
                foreach (AbilityConfigSSO acsso in AbilityConfigs)
                {
                    ag.AbilityNames.Add(acsso.Ability.AbilityName);
                    ag.Abilities.Add(acsso.Ability.Clone());
                }

                return ag;
            }
        }

        public AbilityGroup GetAbilityGroup_NoData()
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