using UnityEngine;
using System.Collections;
using GameCore;
using GameCore.AbilityDataDriven;
using Sirenix.OdinInspector;

namespace Client
{
    public class ProjectileColliderRoot : MonoBehaviour
    {
        [LabelText("碰撞半径")]
        [OnValueChanged("OnRadiusChanged")]
        public float Radius;

        [SerializeField]
        private SphereCollider PlayerCollider;

        [SerializeField]
        private SphereCollider EnemyCollider;

        [SerializeField]
        private SphereCollider SurroundingCollider;

        private void OnRadiusChanged()
        {
            PlayerCollider.radius = Radius;
            EnemyCollider.radius = Radius;
            SurroundingCollider.radius = Radius;
        }

        public void Init(ProjectileInfo projectileInfo)
        {
            PlayerCollider.radius = Radius;
            EnemyCollider.radius = Radius;
            SurroundingCollider.radius = Radius;

            PlayerCollider.enabled = false;
            EnemyCollider.enabled = false;
            SurroundingCollider.enabled = true;
            SurroundingCollider.isTrigger = false;

            if (projectileInfo.ProjectileConfig.CollisionFilter.HasFlag(ENUM_MultipleTargetTeam.UNIT_TARGET_TEAM_ENEMY))
            {
                bool penetrate = projectileInfo.ProjectileConfig.PenetrateFilter.HasFlag(ENUM_MultipleTargetTeam.UNIT_TARGET_TEAM_ENEMY);
                if (projectileInfo.MechaCamp == MechaCamp.Player || projectileInfo.MechaCamp == MechaCamp.Friend)
                {
                    EnemyCollider.enabled = true;
                    EnemyCollider.isTrigger = penetrate;
                }
                else if (projectileInfo.MechaCamp == MechaCamp.Enemy)
                {
                    PlayerCollider.enabled = true;
                    PlayerCollider.isTrigger = penetrate;
                }
            }

            if (projectileInfo.ProjectileConfig.CollisionFilter.HasFlag(ENUM_MultipleTargetTeam.UNIT_TARGET_TEAM_FRIENDLY))
            {
                bool penetrate = projectileInfo.ProjectileConfig.PenetrateFilter.HasFlag(ENUM_MultipleTargetTeam.UNIT_TARGET_TEAM_FRIENDLY);
                if (projectileInfo.MechaCamp == MechaCamp.Player || projectileInfo.MechaCamp == MechaCamp.Friend)
                {
                    PlayerCollider.enabled = true;
                    PlayerCollider.isTrigger = penetrate;
                }
                else if (projectileInfo.MechaCamp == MechaCamp.Enemy)
                {
                    EnemyCollider.enabled = true;
                    EnemyCollider.isTrigger = penetrate;
                }
            }
        }

        public void OnRecycled()
        {
            PlayerCollider.enabled = false;
            EnemyCollider.enabled = false;
            SurroundingCollider.enabled = false;
        }
    }
}