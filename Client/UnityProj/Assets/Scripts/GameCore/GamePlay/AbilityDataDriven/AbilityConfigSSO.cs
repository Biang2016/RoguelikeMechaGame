using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GameCore.AbilityDataDriven
{
    [CreateAssetMenu(menuName = "BattleConfig/Ability")]
    public class AbilityConfigSSO : SerializedScriptableObject
    {
        [NonSerialized]
        [OdinSerialize]
        [ShowInInspector]
        [LabelText("技能")]
        private Ability ability;

        public Ability Ability
        {
            get
            {
                ability.AbilityName = name;
                return ability;
            }
        }

        private string AbilityName => name;
    }
}