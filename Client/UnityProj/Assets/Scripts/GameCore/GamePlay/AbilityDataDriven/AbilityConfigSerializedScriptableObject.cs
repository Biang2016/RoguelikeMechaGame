using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GameCore.AbilityDataDriven
{
    [CreateAssetMenu(menuName = "BattleConfig/AbilityConfig")]
    public class AbilityConfigSerializedScriptableObject : SerializedScriptableObject
    {
        [NonSerialized]
        [OdinSerialize]
        public Ability Ability = new Ability();
    }
}