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
        public Ability Ability = new Ability();

        public string AbilityName => Ability.AbilityName;
    }
}