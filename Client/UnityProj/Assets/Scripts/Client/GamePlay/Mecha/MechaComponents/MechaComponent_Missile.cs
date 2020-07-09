using GameCore;
using UnityEngine;

namespace Client
{
    public class MechaComponent_Missile : MechaComponent_Controllable_Base
    {
        public Shooter Shooter;

        void Start()
        {
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void ControlPerFrame()
        {
            if (Input.GetButton("Fire2"))
            {
                Shooter?.ContinuousShoot();
            }
            else
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    Shooter?.Shoot();
                }
            }
        }
    }
}