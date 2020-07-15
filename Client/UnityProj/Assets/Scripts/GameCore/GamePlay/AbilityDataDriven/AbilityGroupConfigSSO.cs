using System;
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

        public GamePlayAbilityGroup GetAbilityGroup_NoData()
        {
            GamePlayAbilityGroup ag = new GamePlayAbilityGroup();
            ag.AbilityGroupName = AbilityGroupName;
            foreach (AbilityConfigSSO acsso in AbilityConfigs)
            {
                ag.AbilityNames.Add(acsso.Ability.AbilityName);
            }

            return ag;
        }

        [ReadOnly]
        [ShowInInspector]
        private GamePlayAbilityGroup AbilityGroup
        {
            get
            {
                GamePlayAbilityGroup ag = new GamePlayAbilityGroup();
                ag.AbilityGroupName = AbilityGroupName;
                foreach (AbilityConfigSSO acsso in AbilityConfigs)
                {
                    ag.AbilityNames.Add(acsso.Ability.AbilityName);
                    ag.Abilities.Add(acsso.Ability.Clone());
                }

                return ag;
            }
        }
    }
}