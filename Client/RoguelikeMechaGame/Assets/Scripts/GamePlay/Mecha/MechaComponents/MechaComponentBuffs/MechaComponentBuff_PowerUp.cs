public class MechaComponentBuff_PowerUp : MechaComponentBuff_Base
{
    private Modifier My_Modifier = new Modifier(2, Sign.Multiply);

    public override void AddBuff()
    {
        if (Source is IBuff_PowerUp mc)
        {
            mc.AddModifier(My_Modifier);
        }
    }

    public override void DisableBuff()
    {
        if (Source is IBuff_PowerUp mc)
        {
            mc.Remove(My_Modifier);
        }
    }
}

public interface IBuff_PowerUp
{
    void AddModifier(Modifier modifier);
    void Remove(Modifier modifier);
}