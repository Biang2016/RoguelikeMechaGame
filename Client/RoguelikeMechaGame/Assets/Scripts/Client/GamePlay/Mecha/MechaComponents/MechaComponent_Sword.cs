using UnityEngine;
using System.Collections;

namespace Client
{
    public class MechaComponent_Sword : MechaComponentBase
    {
        public Blade Blade;

        protected override void Child_Initialize()
        {
            base.Child_Initialize();
            if (ParentMecha) Blade.Initialize(new BladeInfo(ParentMecha.MechaInfo.MechaType, 0.1f, 30));
        }
    }
}