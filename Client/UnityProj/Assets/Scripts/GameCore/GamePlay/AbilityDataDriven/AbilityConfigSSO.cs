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
        public GamePlayAbility Ability = new GamePlayAbility();

        public string AbilityName => Ability.AbilityName;
    }
}