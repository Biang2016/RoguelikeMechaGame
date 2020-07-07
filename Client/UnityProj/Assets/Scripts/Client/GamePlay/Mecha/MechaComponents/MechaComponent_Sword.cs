namespace Client
{
    public class MechaComponent_Sword : MechaComponentBase
    {
        public Blade Blade;

        protected override void Child_Initialize()
        {
            base.Child_Initialize();
            if (Mecha) Blade.Initialize(new BladeInfo(Mecha.MechaInfo.MechaType, 0.1f, 30));
        }
    }
}