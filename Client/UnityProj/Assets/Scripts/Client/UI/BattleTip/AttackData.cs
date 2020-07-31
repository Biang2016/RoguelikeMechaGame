using GameCore;

namespace Client
{
    public struct AttackData
    {
        public MechaComponentInfo AttackerMCI;
        public MechaComponent HitterMCB;
        public int DecHp;
        public BattleTipType BattleTipType;
        public int ElementType;
        public int ElementHP;

        public AttackData(MechaComponentInfo attackerMCI, MechaComponent hitterMcb, int decHp, BattleTipType battleTipType, int elementType, int elementHp)
        {
            AttackerMCI = attackerMCI;
            HitterMCB = hitterMcb;
            DecHp = decHp;
            BattleTipType = battleTipType;
            ElementType = elementType;
            ElementHP = elementHp;
        }
    }
}