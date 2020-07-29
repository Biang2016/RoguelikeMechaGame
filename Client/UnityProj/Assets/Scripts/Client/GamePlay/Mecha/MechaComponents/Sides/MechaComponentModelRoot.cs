using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Client
{
    public class MechaComponentModelRoot : ForbidLocalMoveRoot
    {
        [SerializeField]
        private List<MechaComponentModel> Models = new List<MechaComponentModel>();

        void Awake()
        {
            Models = GetComponentsInChildren<MechaComponentModel>().ToList();
        }

        public void SetShown(bool shown)
        {
            foreach (MechaComponentModel model in Models)
            {
                model.SetShown(shown);
            }
        }

        public void OnDamage(float portion)
        {
            foreach (MechaComponentModel model in Models)
            {
                model.OnDamage(portion);
            }
        }

        public void OnPowerChange(float portion)
        {
            foreach (MechaComponentModel model in Models)
            {
                model.OnPowerChange(portion);
            }
        }

        public void ResetDefaultEmissionColor(Color highLightEmissionColor)
        {
            foreach (MechaComponentModel model in Models)
            {
                model.ResetDefaultEmissionColor(highLightEmissionColor);
            }
        }

        public void ResetColor()
        {
            foreach (MechaComponentModel model in Models)
            {
                model.ResetColor();
            }
        }
    }
}