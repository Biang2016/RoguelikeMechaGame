using System;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace GameCore
{
    [Category("机甲")]
    [Name("按花名查找机甲组件")]
    [Description("按花名查找机甲组件")]
    public class GetMechaComponentInfoByAlias : CallableFunctionNode<MechaComponentInfo, string>
    {
        public override MechaComponentInfo Invoke(string alias)
        {
            if (Mecha == null || Mecha.MechaInfo == null) return null;
            Mecha.MechaInfo.MechaComponentInfoDict_Alias.TryGetValue(alias, out MechaComponentInfo mci);
            return mci;
        }
    }

    [Category("机甲")]
    [Name("Weapon0攻击")]
    [Description("Weapon0攻击")]
    public class MainWeaponShoot : CallableFunctionNode<bool>
    {
        public override bool Invoke()
        {
            if (Mecha == null || Mecha.MechaInfo == null) return false;
            Mecha.MechaInfo.MechaComponentInfoDict_Alias.TryGetValue("Weapon0", out MechaComponentInfo mci);
            if (mci != null)
            {
                return mci.TriggerSkill();
            }

            return false;
        }
    }

    [Category("机甲")]
    [Name("获取攻击间隔")]
    [Description("获取攻击间隔")]
    public class GetWeaponShootInterval : CallableFunctionNode<float, string>
    {
        public override float Invoke(string weaponName)
        {
            if (Mecha == null || Mecha.MechaInfo == null) return 9999f;

            if (Enum.TryParse<MechaAIConfigParamType>(weaponName + "_AttackInterval", out MechaAIConfigParamType paramType))
            {
                Mecha.MechaInfo.MechaConfig.MechaAIParamDict.TryGetValue(paramType, out float interval);
                return interval;
            }

            return 9999f;
        }
    }

    [Category("机甲")]
    [Name("转向玩家")]
    [Description("转向玩家")]
    public class RotateTowardsMainPlayer : CallableActionNode
    {
        public override void Invoke()
        {
            if (Mecha == null || Mecha.MechaInfo == null) return;
            if (Mecha.MechaInfo.MechaConfig.MechaAIParamDict.TryGetValue(MechaAIConfigParamType.RotateSpeed, out float rotateSpeed))
            {
                Mecha.MechaBaseAIAgent.RotateSpeed = rotateSpeed;
                Mecha.MechaBaseAIAgent.SetRotateTarget(BattleManager.Instance.PlayerMechaInfo.Position);
                Mecha.MechaBaseAIAgent.EnableRotate = true;
            }
            else
            {
                Debug.LogError($"【AI原子】{Mecha.name}不存在AI参数配置{MechaAIConfigParamType.RotateSpeed},请检查AI配置");
            }
        }
    }

    [Category("机甲")]
    [Name("朝玩家移动")]
    [Description("朝玩家移动")]
    public class MoveTowardsMainPlayer : CallableActionNode
    {
        public override void Invoke()
        {
            if (Mecha == null || Mecha.MechaInfo == null) return;
            if (Mecha.MechaInfo.MechaConfig.MechaAIParamDict.TryGetValue(MechaAIConfigParamType.MoveSpeed, out float moveSpeed))
            {
                Mecha.MechaBaseAIAgent.MoveSpeed = moveSpeed;
                Mecha.MechaBaseAIAgent.SetDestination(BattleManager.Instance.PlayerMechaInfo.Position);
                Mecha.MechaBaseAIAgent.EnableMove = true;
            }
            else
            {
                Debug.LogError($"【AI原子】{Mecha.name}不存在AI参数配置{MechaAIConfigParamType.MoveSpeed},请检查AI配置");
            }
        }
    }

    [Category("机甲")]
    [Name("朝玩家移动并保持距离")]
    [Description("朝玩家移动并保持距离")]
    public class MoveTowardsMainPlayerWithMinDistance : CallableActionNode
    {
        public override void Invoke()
        {
            if (Mecha == null || Mecha.MechaInfo == null) return;
            if (Mecha.MechaInfo.MechaConfig.MechaAIParamDict.TryGetValue(MechaAIConfigParamType.MoveSpeed, out float moveSpeed))
            {
                if (Mecha.MechaInfo.MechaConfig.MechaAIParamDict.TryGetValue(MechaAIConfigParamType.AttackDistance, out float attackDistance))
                {
                    Mecha.MechaBaseAIAgent.MoveSpeed = moveSpeed;
                    Vector3 diff = Mecha.MechaInfo.Position - BattleManager.Instance.PlayerMechaInfo.Position;
                    Mecha.MechaBaseAIAgent.SetDestination((BattleManager.Instance.PlayerMechaInfo.Position + diff.normalized * attackDistance));
                    Mecha.MechaBaseAIAgent.EnableMove = true;
                }
                else
                {
                    Debug.LogError($"【AI原子】{Mecha.name}不存在AI参数配置{MechaAIConfigParamType.AttackDistance},请检查AI配置");
                }
            }
            else
            {
                Debug.LogError($"【AI原子】{Mecha.name}不存在AI参数配置{MechaAIConfigParamType.MoveSpeed},请检查AI配置");
            }
        }
    }
}