using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GameCore.AbilityDataDriven
{
    [CreateAssetMenu(menuName = "BattleConfig/MechaComponentGroup")]
    public class MechaComponentGroupConfigSSO : SerializedScriptableObject
    {
        [NonSerialized]
        [OdinSerialize]
        public GamePlayAbility Ability = new GamePlayAbility();

        public string AbilityName => Ability.AbilityName;
    }
}