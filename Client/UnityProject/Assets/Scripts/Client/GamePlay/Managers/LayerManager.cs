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
        public int LayerMask_Projectile_CollideSurroundings;
        public int LayerMask_Projectile_CollidePlayer;
        public int LayerMask_Projectile_CollideEnemy;
        public int LayerMask_BackpackItemGrid;
        public int LayerMask_DragAreas;
        public int LayerMask_ItemDropped;

        public int Layer_UI;
        public int Layer_ComponentHitBox_Player;
        public int Layer_ComponentHitBox_Enemy;
        public int Layer_Projectile_CollideSurroundings;
        public int Layer_Projectile_CollidePlayer;
        public int Layer_Projectile_CollideEnemy;
        public int Layer_BackpackItemGrid;
        public int Layer_DragAreas;
        public int Layer_ItemDropped;

        public override void Awake()
        {
            LayerMask_UI = LayerMask.GetMask("UI");
            LayerMask_ComponentHitBox_Player = LayerMask.GetMask("ComponentHitBox_Player");
            LayerMask_ComponentHitBox_Enemy = LayerMask.GetMask("ComponentHitBox_Enemy");
            LayerMask_Projectile_CollideSurroundings = LayerMask.GetMask("Projectile_CollideSurroundings");
            LayerMask_Projectile_CollidePlayer = LayerMask.GetMask("Projectile_CollidePlayer");
            LayerMask_Projectile_CollideEnemy = LayerMask.GetMask("Projectile_CollideEnemy");
            LayerMask_BackpackItemGrid = LayerMask.GetMask("BackpackItemGrid");
            LayerMask_DragAreas = LayerMask.GetMask("DragAreas");
            LayerMask_ItemDropped = LayerMask.GetMask("ItemDropped");

            Layer_UI = LayerMask.NameToLayer("UI");
            Layer_ComponentHitBox_Player = LayerMask.NameToLayer("ComponentHitBox_Player");
            Layer_ComponentHitBox_Enemy = LayerMask.NameToLayer("ComponentHitBox_Enemy");
            Layer_Projectile_CollideSurroundings = LayerMask.NameToLayer("Projectile_CollideSurroundings");
            Layer_Projectile_CollidePlayer = LayerMask.NameToLayer("Projectile_CollidePlayer");
            Layer_Projectile_CollideEnemy = LayerMask.NameToLayer("Projectile_CollideEnemy");
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