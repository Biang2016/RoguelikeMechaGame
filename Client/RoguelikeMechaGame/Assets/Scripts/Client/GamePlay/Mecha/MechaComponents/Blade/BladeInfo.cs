using System.Collections.Generic;
using GameCore;

namespace Client
{
    public class BladeInfo
    {
        public MechaType MechaType;
        public int Damage;
        public List<Modifier> Modifiers_Damage = new List<Modifier>();

        public float Interval;
        public List<Modifier> Modifiers_Interval = new List<Modifier>();

        public BladeInfo(MechaType mechaType, float interval, int damage)
        {
            MechaType = mechaType;
            Interval = interval;
            Damage = damage;
        }

        public int FinalDamage => Modifiers_Damage.CalculateModifiers(Damage);
        public float FinalInterval => Modifiers_Interval.CalculateModifiers(Interval);
    }
}