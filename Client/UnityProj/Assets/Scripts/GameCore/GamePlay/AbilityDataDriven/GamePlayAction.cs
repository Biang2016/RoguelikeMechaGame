using System;
using System.Collections;
using System.Collections.Generic;
using BiangStudio.CloneVariant;
using BiangStudio.Messenger;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace GameCore.AbilityDataDriven
{
    public struct ExecuteInfo
    {
        public Ability Ability;
        public MechaInfo MechaInfo;
        public MechaComponentInfo MechaComponentInfo;

        public ExecuteInfo(Ability ability, MechaInfo mechaInfo, MechaComponentInfo mechaComponentInfo)
        {
            Ability = ability;
            MechaInfo = mechaInfo;
            MechaComponentInfo = mechaComponentInfo;
        }
    }

    public abstract class GamePlayAction : IClone<GamePlayAction>
    {
        [HideInInspector]
        public string ActionName => GetType().ToString();

        public GamePlayAction Clone()
        {
            Type type = GetType();
            GamePlayAction newConfig = (GamePlayAction) Activator.CreateInstance(type);
            ChildClone(newConfig);
            return newConfig;
        }

        protected virtual void ChildClone(GamePlayAction newConfig)
        {
        }

        public virtual void OnRegisterEvent(Messenger messenger, ENUM_AbilityEvent abilityEvent, Ability parentAbility)
        {
            messenger.AddListener<ExecuteInfo>((uint) abilityEvent, (executeInfo) =>
            {
                if (parentAbility == executeInfo.Ability)
                {
                }
            });
        }
    }

    [LabelText("行为_赋予技能")]
    public class Action_AddAbility : GamePlayAction
    {
        public GamePlayActionTarget Target;

        [LabelText("技能名称")]
        public string AbilityName;

        protected override void ChildClone(GamePlayAction newConfig)
        {
            base.ChildClone(newConfig);
            Action_AddAbility action = ((Action_AddAbility) newConfig);
            action.Target = Target.Clone();
            action.AbilityName = AbilityName;
        }
    }

    public class Action_ActOnTargets : GamePlayAction
    {
        public GamePlayActionTarget Target;

        public GamePlayAction Action;

        protected override void ChildClone(GamePlayAction newConfig)
        {
            base.ChildClone(newConfig);
            Action_ActOnTargets action = ((Action_ActOnTargets) newConfig);
            action.Target = Target.Clone();
            action.Action = Action.Clone();
        }
    }

    [LabelText("行为_赋予Modifier")]
    public class Action_ApplyModifier : GamePlayAction
    {
        public GamePlayActionTarget Target;

        [LabelText("Modifier名称")]
        public string ModifierName;

        protected override void ChildClone(GamePlayAction newConfig)
        {
            base.ChildClone(newConfig);
            Action_ApplyModifier action = ((Action_ApplyModifier) newConfig);
            action.Target = Target.Clone();
            action.ModifierName = ModifierName;
        }
    }

    public class Action_EmitProjectile : GamePlayAction
    {
        [PropertyOrder(-1)]
        [ValueDropdown("GetProjectileNames")]
        public string ProjectileName;

        private ProjectileConfig projectConfig;

        [PropertyOrder(-1)]
        [LabelText("投掷物")]
        [ReadOnly]
        [ShowInInspector]
        public ProjectileConfig ProjectileConfig
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ProjectileName))
                {
                    projectConfig = null;
                    return null;
                }
                else
                {
                    if (projectConfig == null)
                    {
                        projectConfig = ConfigManager.Instance.GetProjectileConfig(ProjectileName);
                    }
                }

                return projectConfig;
            }
        }

        private IEnumerable GetProjectileNames()
        {
            if (!ConfigManager.IsLoaded)
            {
                ConfigManager.LoadAllConfigs();
            }

            return ConfigManager.ProjectileConfigDict.Keys;
        }

        public override void OnRegisterEvent(Messenger messenger, ENUM_AbilityEvent abilityEvent, Ability parentAbility)
        {
            messenger.AddListener<ExecuteInfo>((uint) abilityEvent, (executeInfo) =>
            {
                if (parentAbility == executeInfo.Ability)
                {
                    Execute(executeInfo);
                }
            });
        }

        public void Execute(ExecuteInfo executeInfo)
        {
            ProjectileInfo pi = new ProjectileInfo(this, executeInfo, null, Vector3.zero);
            ProjectileManager.Instance.EmitProjectileHandler(pi);
        }

        protected override void ChildClone(GamePlayAction newConfig)
        {
            base.ChildClone(newConfig);
            Action_EmitProjectile action = ((Action_EmitProjectile) newConfig);
            action.ProjectileName = ProjectileName;
        }
    }

    [LabelText("行为_造成伤害")]
    public class Action_DealDamage : GamePlayAction
    {
        public GamePlayActionTarget Target;

        [LabelText("伤害量")]
        public int Damage;

        protected override void ChildClone(GamePlayAction newConfig)
        {
            base.ChildClone(newConfig);
            Action_DealDamage action = ((Action_DealDamage) newConfig);
            action.Target = Target.Clone();
            action.Damage = Damage;
        }

        public override void OnRegisterEvent(Messenger messenger, ENUM_AbilityEvent abilityEvent, Ability parentAbility)
        {
            // Projectile Damage
            messenger.AddListener<ExecuteInfo, ProjectileInfo.FlyRealtimeData>((uint) abilityEvent, (executeInfo, flyRealTimeData) =>
            {
                ExecuteProjectileDamage(parentAbility, executeInfo, flyRealTimeData);
            });
        }

        public void ExecuteProjectileDamage(Ability parentAbility, ExecuteInfo executeInfo, ProjectileInfo.FlyRealtimeData flyRealTimeData)
        {
            if (parentAbility == executeInfo.Ability)
            {
                int finalDamage = Damage;
                switch (executeInfo.MechaComponentInfo.CurrentPowerUpgradeData)
                {
                    case PowerUpgradeData_Gun pud_Gun:
                    {
                        finalDamage += Mathf.FloorToInt(pud_Gun.DamageIncreasePercent / 100f * Damage);
                        break;
                    }
                }

                switch (Target)
                {
                    case SingleActionTarget singleTarget:
                    {
                        switch (singleTarget.Target)
                        {
                            case ENUM_SingleTarget.ATTACKER:
                            {
                                break;
                            }
                            case ENUM_SingleTarget.CASTER:
                            {
                                if (flyRealTimeData.HitMechaComponentInfo != null)
                                {
                                    if (flyRealTimeData.HitMechaComponentInfo == executeInfo.MechaComponentInfo && flyRealTimeData.HitMechaComponentInfo.CheckAlive())
                                    {
                                        Debug.Log($"{executeInfo.MechaComponentInfo.LogIdentityName} Dealt <color=\"#FF585F\">{finalDamage}</color> damage to <color=\"#FF74FF\">{singleTarget.Target.ToString()}</color> {flyRealTimeData.HitMechaComponentInfo.LogIdentityName}");
                                        flyRealTimeData.HitMechaComponentInfo.Damage(executeInfo.MechaComponentInfo, finalDamage);
                                    }
                                }

                                break;
                            }
                            case ENUM_SingleTarget.POINT:
                            {
                                break;
                            }
                            case ENUM_SingleTarget.TARGET:
                            {
                                break;
                            }
                            case ENUM_SingleTarget.UNIT:
                            {
                                if (flyRealTimeData.HitMechaComponentInfo != null && flyRealTimeData.HitMechaComponentInfo.MechaInfo != executeInfo.MechaInfo && flyRealTimeData.HitMechaComponentInfo.CheckAlive())
                                {
                                    Debug.Log($"{executeInfo.MechaComponentInfo.LogIdentityName} Dealt <color=\"#FF585F\">{finalDamage}</color> damage to <color=\"#FF74FF\">{singleTarget.Target.ToString()}</color> {flyRealTimeData.HitMechaComponentInfo.LogIdentityName}");
                                    flyRealTimeData.HitMechaComponentInfo.Damage(executeInfo.MechaComponentInfo, finalDamage);
                                }

                                break;
                            }
                        }

                        break;
                    }
                    case MultipleActionTarget multipleTarget:
                    {
                        switch (multipleTarget.Center)
                        {
                            case ENUM_MultipleTargetCenter.PROJECTILE:
                            {
                                List<MechaComponentInfo> hitMCIs = BattleManager.Instance.SearchRangeHandler(flyRealTimeData.Position, multipleTarget.Radius);
                                foreach (MechaComponentInfo mci in hitMCIs)
                                {
                                    mci.Damage(executeInfo.MechaComponentInfo, finalDamage);
                                }

                                break;
                            }
                        }

                        break;
                    }
                }
            }
        }
    }
}