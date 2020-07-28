using System;
using GameCore.AbilityDataDriven;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GameCore
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