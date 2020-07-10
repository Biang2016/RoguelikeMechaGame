using UnityEngine;

namespace Client
{
    public class MechaComponent_Gun : MechaComponent_Controllable_Base
    {
        public Shooter Shooter;

        protected override void Child_Initialize()
        {
            base.Child_Initialize();
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void ControlPerFrame()
        {
            //if (ControlManager.Instance.CheckButtonAction(Shooter.ShooterInfo.TriggerButtonState))
            //{
            //    Shooter?.ContinuousShoot();
            //}
        }

        public void AddModifier(Modifier modifier)
        {
            Shooter.ShooterInfo.ProjectileInfo.Modifiers_Damage.Add(modifier);
        }

        public void RemoveModifier(Modifier modifier)
        {
            Shooter.ShooterInfo.ProjectileInfo.Modifiers_Damage.Remove(modifier);
        }
    }
}