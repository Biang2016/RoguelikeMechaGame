using BiangStudio.GameDataFormat;

namespace Client
{
    public class MechaComponent_Sword : MechaComponentBase
    {
        public Blade Blade;

        protected override void Child_Initialize()
        {
            base.Child_Initialize();
            if (Mecha) Blade.Initialize(new BladeInfo(Mecha.MechaInfo.MechaType, (Fix64) 0.1f, 30));
        }
    }
}