using System;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace GameCore
{
    public static class FlowExtensions
    {
        public static MechaBase GetMechaBase(this SimplexNode node)
        {
            MechaBase mecha = node.parentNode.graph.agent.GetComponent<MechaBase>();
            return mecha;
        }
    }

    [Category("MechaComponent")]
    [Name("按花名查找机甲组件")]
    public class GetMechaComponentInfoByAlias : CallableFunctionNode<MechaComponentInfo, string>
    {
        public override MechaComponentInfo Invoke(string alias)
        {
            MechaBase mecha = this.GetMechaBase();
            if (mecha == null || mecha.MechaInfo == null) return null;
            mecha.MechaInfo.MechaComponentInfoDict_Alias.TryGetValue(alias, out MechaComponentInfo mci);
            return mci;
        }
    }

    [Category("MechaComponent")]
    [Name("Weapon0射击")]
    public class MainWeaponShoot : CallableFunctionNode<bool>
    {
        public override bool Invoke()
        {
            MechaBase mecha = this.GetMechaBase();
            if (mecha == null || mecha.MechaInfo == null) return false;
            mecha.MechaInfo.MechaComponentInfoDict_Alias.TryGetValue("Weapon0", out MechaComponentInfo mci);
            if (mci != null)
            {
                return mci.TriggerSkill();
            }

            return false;
        }
    }

    [Category("MechaComponent")]
    [Name("获取射击间隔")]
    public class GetWeaponShootInterval : CallableFunctionNode<float, string>
    {
        public override float Invoke(string weaponName)
        {
            MechaBase mecha = this.GetMechaBase();
            if (mecha == null || mecha.MechaInfo == null) return 9999f;

            if (Enum.TryParse<MechaAIConfigParamType>(weaponName + "_AttackInterval", out MechaAIConfigParamType paramType))
            {
                mecha.MechaInfo.MechaConfig.MechaAIParamDict.TryGetValue(paramType, out float interval);
                return interval;
            }

            return 9999f;
        }
    }

    [Category("MechaComponent")]
    [Name("转向玩家")]
    public class RotateTowardsMainPlayer : CallableActionNode
    {
        public override void Invoke()
        {
            MechaBase mecha = this.GetMechaBase();
            if (mecha == null || mecha.MechaInfo == null) return;
            if (mecha.MechaInfo.MechaConfig.MechaAIParamDict.TryGetValue(MechaAIConfigParamType.RotateSpeed, out float rotateSpeed))
            {
                mecha.MechaBaseAIAgent.RotateSpeed = rotateSpeed;
                mecha.MechaBaseAIAgent.SetRotateTarget(BattleManager.Instance.PlayerMechaInfo.Position);
                mecha.MechaBaseAIAgent.EnableRotate = true;
            }
            else
            {
                Debug.LogError($"【AI原子】{mecha.name}不存在AI参数配置{MechaAIConfigParamType.RotateSpeed},请检查AI配置");
            }
        }

        [Category("MechaComponent")]
        [Name("朝玩家移动")]
        public class MoveTowardsMainPlayer : CallableActionNode
        {
            public override void Invoke()
            {
                MechaBase mecha = this.GetMechaBase();
                if (mecha == null || mecha.MechaInfo == null) return;
                if (mecha.MechaInfo.MechaConfig.MechaAIParamDict.TryGetValue(MechaAIConfigParamType.MoveSpeed, out float moveSpeed))
                {
                    mecha.MechaBaseAIAgent.MoveSpeed = moveSpeed;
                    mecha.MechaBaseAIAgent.SetDestination(BattleManager.Instance.PlayerMechaInfo.Position);
                    mecha.MechaBaseAIAgent.EnableMove = true;
                }
                else
                {
                    Debug.LogError($"【AI原子】{mecha.name}不存在AI参数配置{MechaAIConfigParamType.MoveSpeed},请检查AI配置");
                }
            }
        }

        [Category("MechaComponent")]
        [Name("朝玩家移动并保持距离")]
        public class MoveTowardsMainPlayerWithMinDistance : CallableActionNode
        {
            public override void Invoke()
            {
                MechaBase mecha = this.GetMechaBase();
                if (mecha == null || mecha.MechaInfo == null) return;
                if (mecha.MechaInfo.MechaConfig.MechaAIParamDict.TryGetValue(MechaAIConfigParamType.MoveSpeed, out float moveSpeed))
                {
                    if (mecha.MechaInfo.MechaConfig.MechaAIParamDict.TryGetValue(MechaAIConfigParamType.AttackDistance, out float attackDistance))
                    {
                        mecha.MechaBaseAIAgent.MoveSpeed = moveSpeed;
                        Vector3 diff = mecha.MechaInfo.Position - BattleManager.Instance.PlayerMechaInfo.Position;  
                        mecha.MechaBaseAIAgent.SetDestination((BattleManager.Instance.PlayerMechaInfo.Position + diff.normalized * attackDistance));
                        mecha.MechaBaseAIAgent.EnableMove = true;
                    }
                    else
                    {
                        Debug.LogError($"【AI原子】{mecha.name}不存在AI参数配置{MechaAIConfigParamType.AttackDistance},请检查AI配置");
                    }
                }
                else
                {
                    Debug.LogError($"【AI原子】{mecha.name}不存在AI参数配置{MechaAIConfigParamType.MoveSpeed},请检查AI配置");
                }
            }
        }
    }
}