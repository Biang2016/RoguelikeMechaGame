using System.Collections.Generic;
using BiangStudio.CloneVariant;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore.AbilityDataDriven
{
    public class GamePlayAbilityGroup : IClone<GamePlayAbilityGroup>
    {
        public string AbilityGroupName;

        [HideInInspector]
        public List<string> AbilityNames = new List<string>();

        [ListDrawerSettings(ListElementLabelName = "AbilityName")]
        public List<GamePlayAbility> Abilities = new List<GamePlayAbility>();

        public GamePlayAbilityGroup Clone()
        {
            GamePlayAbilityGroup ag = new GamePlayAbilityGroup();
            ag.AbilityGroupName = AbilityGroupName;
            ag.AbilityNames = AbilityNames.Clone();
            ag.Abilities = Abilities.Clone();
            return ag;
        }
    }
}