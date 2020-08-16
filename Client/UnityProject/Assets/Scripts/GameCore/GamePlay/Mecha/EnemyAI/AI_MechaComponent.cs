using FlowCanvas.Nodes;
using ParadoxNotion.Design;

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
    [Name("主武器射击")]
    public class MainWeaponShoot : CallableFunctionNode<bool>
    {
        public override bool Invoke()
        {
            MechaBase mecha = this.GetMechaBase();
            if (mecha == null || mecha.MechaInfo == null) return false;
            mecha.MechaInfo.MechaComponentInfoDict_Alias.TryGetValue("MainWeapon", out MechaComponentInfo mci);
            if (mci != null)
            {
                return mci.TriggerSkill();
            }

            return false;
        }
    }

    [Category("MechaComponent")]
    [Name("转向玩家")]
    public class RotateTowardsMainPlayer : CallableActionNode<float>
    {
        public override void Invoke(float rotateSpeed)
        {
            MechaBase mecha = this.GetMechaBase();
            if (mecha == null || mecha.MechaInfo == null) return;
            mecha.MechaBaseAIAgent.RotateSpeed = rotateSpeed;
            mecha.MechaBaseAIAgent.SetRotateTarget(BattleManager.Instance.PlayerMechaInfo.Position);
            mecha.MechaBaseAIAgent.EnableRotate = true;
        }
    }

    [Category("MechaComponent")]
    [Name("朝玩家移动")]
    public class MoveTowardsMainPlayer : CallableActionNode<float>
    {
        public override void Invoke(float moveSpeed)
        {
            MechaBase mecha = this.GetMechaBase();
            if (mecha == null || mecha.MechaInfo == null) return;
            mecha.MechaBaseAIAgent.MoveSpeed = moveSpeed;
            mecha.MechaBaseAIAgent.SetDestination(BattleManager.Instance.PlayerMechaInfo.Position);
            mecha.MechaBaseAIAgent.EnableMove = true;
        }
    }
}