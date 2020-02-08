public interface IBuff
{
    void AddModifier(Modifier modifier);
    void RemoveModifier(Modifier modifier);
}

public interface IBuff_PowerUp : IBuff
{
}