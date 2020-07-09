using System;
using System.Collections.Generic;
using GameCore;

namespace Client
{
    [Serializable]
    public class ProjectileInfo
    {
        public MechaType MechaType;
        public ProjectileType ProjectileType;
        public int Damage;
        public List<Modifier> Modifiers_Damage = new List<Modifier>();

        public float Speed;
        public List<Modifier> Modifiers_Speed = new List<Modifier>();

        public ProjectileInfo(MechaType mechaType, ProjectileType projectileType, float speed, int damage)
        {
            MechaType = mechaType;
            ProjectileType = projectileType;
            Speed = speed;
            Damage = damage;
        }

        public int FinalDamage => Modifiers_Damage.CalculateModifiers(Damage);
        public float FinalSpeed => Modifiers_Speed.CalculateModifiers(Speed);
    }
}