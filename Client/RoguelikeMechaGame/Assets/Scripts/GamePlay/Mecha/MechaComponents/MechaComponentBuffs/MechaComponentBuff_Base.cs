public abstract class MechaComponentBuff_Base
{
    internal MechaComponentBase Source;
    internal MechaComponentBase Target;

    public virtual void Initialize(MechaComponentBase source, MechaComponentBase target)
    {
        Source = source;
        Target = target;
    }

    public abstract void AddBuff();
    public abstract void DisableBuff();
}