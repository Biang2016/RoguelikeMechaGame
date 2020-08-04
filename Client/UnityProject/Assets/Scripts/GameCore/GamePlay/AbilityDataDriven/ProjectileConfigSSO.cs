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
        [ShowInInspector]
        [LabelText("投掷物配置")]
        private ProjectileConfig projectileConfig = new ProjectileConfig();

        public ProjectileConfig ProjectileConfig
        {
            get
            {
                projectileConfig.ProjectileName = name;
                return projectileConfig;
            }
        }
    }
}