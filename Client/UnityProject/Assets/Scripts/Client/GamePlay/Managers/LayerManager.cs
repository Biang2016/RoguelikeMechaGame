using BiangStudio.Singleton;
using GameCore;
using GameCore.AbilityDataDriven;
using UnityEngine;

namespace Client
{
    public class LayerManager : TSingletonBaseManager<LayerManager>
    {
        public int LayerMask_UI;
        public int LayerMask_ComponentHitBox_Player;
        public int LayerMask_ComponentHitBox_Enemy;
        public int LayerMask_Projectile_Both;
        public int LayerMask_Projectile_PlayerOnly;
        public int LayerMask_Projectile_EnemyOnly;
        public int LayerMask_BackpackItemGrid;
        public int LayerMask_DragAreas;
        public int LayerMask_ItemDropped;

        public int Layer_UI;
        public int Layer_ComponentHitBox_Player;
        public int Layer_ComponentHitBox_Enemy;
        public int Layer_Projectile_Both;
        public int Layer_Projectile_PlayerOnly;
        public int Layer_Projectile_EnemyOnly;
        public int Layer_BackpackItemGrid;
        public int Layer_DragAreas;
        public int Layer_ItemDropped;

        public override void Awake()
        {
            LayerMask_UI = LayerMask.GetMask("UI");
            LayerMask_ComponentHitBox_Player = LayerMask.GetMask("ComponentHitBox_Player");
            LayerMask_ComponentHitBox_Enemy = LayerMask.GetMask("ComponentHitBox_Enemy");
            LayerMask_Projectile_Both = LayerMask.GetMask("Projectile_Both");
            LayerMask_Projectile_PlayerOnly = LayerMask.GetMask("Projectile_PlayerOnly");
            LayerMask_Projectile_EnemyOnly = LayerMask.GetMask("Projectile_EnemyOnly");
            LayerMask_BackpackItemGrid = LayerMask.GetMask("BackpackItemGrid");
            LayerMask_DragAreas = LayerMask.GetMask("DragAreas");
            LayerMask_ItemDropped = LayerMask.GetMask("ItemDropped");

            Layer_UI = LayerMask.NameToLayer("UI");
            Layer_ComponentHitBox_Player = LayerMask.NameToLayer("ComponentHitBox_Player");
            Layer_ComponentHitBox_Enemy = LayerMask.NameToLayer("ComponentHitBox_Enemy");
            Layer_Projectile_Both = LayerMask.NameToLayer("Projectile_Both");
            Layer_Projectile_PlayerOnly = LayerMask.NameToLayer("Projectile_PlayerOnly");
            Layer_Projectile_EnemyOnly = LayerMask.NameToLayer("Projectile_EnemyOnly");
            Layer_BackpackItemGrid = LayerMask.NameToLayer("BackpackItemGrid");
            Layer_DragAreas = LayerMask.NameToLayer("DragAreas");
            Layer_ItemDropped = LayerMask.NameToLayer("ItemDropped");
        }

        public int GetLayerMaskByMultipleTargetTeam(ENUM_MultipleTargetTeam team)
        {
            switch (team)
            {
                case ENUM_MultipleTargetTeam.UNIT_TARGET_TEAM_NONE:
                {
                    return 0;
                }
                case ENUM_MultipleTargetTeam.UNIT_TARGET_TEAM_FRIENDLY:
                {
                    return LayerMask_ComponentHitBox_Player;
                }
                case ENUM_MultipleTargetTeam.UNIT_TARGET_TEAM_ENEMY:
                {
                    return LayerMask_ComponentHitBox_Enemy;
                }
                case ENUM_MultipleTargetTeam.UNIT_TARGET_TEAM_BOTH:
                {
                    return LayerMask_ComponentHitBox_Player | LayerMask_ComponentHitBox_Enemy;
                }
            }

            return 0;
        }

        public int GetLayerByMechaCamp(MechaCamp camp)
        {
            switch (camp)
            {
                case MechaCamp.None:
                {
                    return 0;
                }
                case MechaCamp.Friend:
                {
                    return Layer_ComponentHitBox_Player;
                }
                case MechaCamp.Player:
                {
                    return Layer_ComponentHitBox_Player;
                }
                case MechaCamp.Enemy:
                {
                    return Layer_ComponentHitBox_Enemy;
                }
            }

            return 0;
        }
    }
}