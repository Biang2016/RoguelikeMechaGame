using NodeCanvas.BehaviourTrees;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace GameCore
{
    public static class NodeExtensions
    {
        [Category("机甲")]
        [Name("转向玩家")]
        [Description("转向玩家")]
        public class TurnToMainPlayer : BTNode
        {
            protected override Status OnExecute(Component agent, IBlackboard blackboard)
            {
                if (Mecha == null || Mecha.MechaInfo == null) return Status.Failure;
                if (Mecha.MechaInfo.MechaConfig.MechaAIParamDict.TryGetValue(MechaAIConfigParamType.RotateSpeed, out float rotateSpeed))
                {
                    Mecha.MechaBaseAIAgent.RotateSpeed = rotateSpeed;
                    Mecha.MechaBaseAIAgent.SetRotateTarget(BattleManager.Instance.PlayerMechaInfo.Position);
                    Mecha.MechaBaseAIAgent.EnableRotate = true;
                    return Status.Success;
                }
                else
                {
                    Debug.LogError($"【AI原子】{Mecha.name}不存在AI参数配置{MechaAIConfigParamType.RotateSpeed},请检查AI配置");
                    return Status.Failure;
                }
            }
        }

        [Category("机甲")]
        [Name("朝玩家移动")]
        [Description("朝玩家移动")]
        public class MoveToMainPlayer : BTNode
        {
            [Name("默认保持距离")]
            public BBParameter<float> KeepDistance;

            [Name("启用AI配置参数覆盖")]
            public BBParameter<bool> UseAIParam;

            [Name("AI配置参数")]
            public BBParameter<MechaAIConfigParamType> AIParam;

            protected override Status OnExecute(Component agent, IBlackboard blackboard)
            {
                if (Mecha == null || Mecha.MechaInfo == null) return Status.Failure;
                float attackDistance = KeepDistance.value;
                if (UseAIParam.value)
                {
                    if (!Mecha.MechaInfo.MechaConfig.MechaAIParamDict.TryGetValue(MechaAIConfigParamType.AttackDistance, out attackDistance))
                    {
                        Debug.LogError($"【AI原子】{Mecha.name}不存在AI参数配置{MechaAIConfigParamType.AttackDistance},请检查AI配置");
                    }
                }

                if (Mecha.MechaInfo.MechaConfig.MechaAIParamDict.TryGetValue(MechaAIConfigParamType.MoveSpeed, out float moveSpeed))
                {
                    Mecha.MechaBaseAIAgent.MoveSpeed = moveSpeed;
                    Vector3 diff = Mecha.MechaInfo.Position - BattleManager.Instance.PlayerMechaInfo.Position;
                    Mecha.MechaBaseAIAgent.SetDestination((BattleManager.Instance.PlayerMechaInfo.Position + diff.normalized * attackDistance));
                    Mecha.MechaBaseAIAgent.EnableMove = true;
                    return Status.Success;
                }
                else
                {
                    Debug.LogError($"【AI原子】{Mecha.name}不存在AI参数配置{MechaAIConfigParamType.MoveSpeed},请检查AI配置");
                    return Status.Failure;
                }
            }
        }

        [Category("机甲")]
        [Name("武器攻击")]
        [Description("武器攻击")]
        public class WeaponAttackAction : BTNode
        {
            [Name("武器花名")]
            public BBParameter<string> WeaponAlias;

            protected override Status OnExecute(Component agent, IBlackboard blackboard)
            {
                if (Mecha == null || Mecha.MechaInfo == null) return Status.Failure;
                Mecha.MechaInfo.MechaComponentInfoDict_Alias.TryGetValue(WeaponAlias.value, out MechaComponentInfo mci);
                if (mci != null)
                {
                    mci.TriggerSkill();
                    return Status.Success;
                }

                return Status.Failure;
            }
        }
    }
}