using System.Collections.Generic;
using BiangStudio.GameDataFormat;
using GameCore;

namespace Client
{
    public class BladeInfo
    {
        public MechaType MechaType;
        public int Damage;
        public List<Modifier> Modifiers_Damage = new List<Modifier>();

        public Fix64 Interval;
        public List<Modifier> Modifiers_Interval = new List<Modifier>();

        public BladeInfo(MechaType mechaType, Fix64 interval, int damage)
        {
            MechaType = mechaType;
            Interval = interval;
            Damage = damage;
        }

        public int FinalDamage => Modifiers_Damage.CalculateModifiers(Damage);
        public Fix64 FinalInterval => Modifiers_Interval.CalculateModifiers(Interval);
    }
}