using System;

public class MechaComponentBuff
{
    internal MechaComponentBase Source = null;
    internal MechaComponentBase Target = null;
    internal IBuff TargetIBuff = null;

    protected Modifier Modifier;

    internal Type BuffType;

    public MechaComponentBuff(Type buffTYpe, MechaComponentBase source, MechaComponentBase target, Modifier modifier)
    {
        BuffType = buffTYpe;
        if (target.GetType().GetInterface(BuffType.Name) != null)
        {
            TargetIBuff = (IBuff) target;
        }

        Source = source;
        Target = target;

        Modifier = modifier;
    }

    public void AddBuff()
    {
        Source.GiveOutBuffs.Add(this);
        Target.AttachedBuffs.Add(this);
        TargetIBuff?.AddModifier(Modifier);
    }

    public void RemoveBuff()
    {
        Source.GiveOutBuffs.Remove(this);
        Target.AttachedBuffs.Remove(this);
        TargetIBuff?.RemoveModifier(Modifier);
    }
}