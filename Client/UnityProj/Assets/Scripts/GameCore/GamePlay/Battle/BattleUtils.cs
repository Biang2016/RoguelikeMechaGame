using System;
using System.Collections.Generic;
using BiangStudio.GameDataFormat;

namespace GameCore
{
    public static class BattleUtils
    {
        public static Fix64 CalculateModifiers(this List<Modifier> modifiers, Fix64 value)
        {
            Fix64 res = value;
            foreach (Modifier modifier in modifiers)
            {
                res = modifier.Calculate(res);
            }

            return res;
        }
    }
}