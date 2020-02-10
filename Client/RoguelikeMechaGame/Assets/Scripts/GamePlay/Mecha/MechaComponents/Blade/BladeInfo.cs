using System.Collections.Generic;

public class BladeInfo
{
    public MechaType MechaType;
    public int Damage;
    public List<Modifier> Modifiers_Damage = new List<Modifier>();

    public float Speed;
    public List<Modifier> Modifiers_Speed = new List<Modifier>();

    public BladeInfo(MechaType mechaType, float speed, int damage)
    {
        MechaType = mechaType;
        Speed = speed;
        Damage = damage;
    }

    public int FinalDamage => Modifiers_Damage.CalculateModifiers(Damage);
    public float FinalSpeed => Modifiers_Speed.CalculateModifiers(Speed);
}