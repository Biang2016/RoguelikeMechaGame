using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GameCore.AbilityDataDriven
{
    [CreateAssetMenu(menuName = "BattleConfig/Projectile")]
    public class ProjectileConfigSSO : SerializedScriptableObject
    {
        [NonSerialized]
        [OdinSerialize]
        public ProjectileConfig ProjectileConfig = new ProjectileConfig();

        public string ProjectileName => ProjectileConfig.ProjectileName;
    }
}