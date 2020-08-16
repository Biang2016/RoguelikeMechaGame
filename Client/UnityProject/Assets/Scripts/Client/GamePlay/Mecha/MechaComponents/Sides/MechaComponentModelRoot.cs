using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Client
{
    public class MechaComponentModelRoot : ForbidLocalMoveRoot
    {
        [SerializeField]
        private List<MechaComponentModel> Models = new List<MechaComponentModel>();

        private Animator ModelAnimator;

        void Awake()
        {
            Models = GetComponentsInChildren<MechaComponentModel>().ToList();
            ModelAnimator = GetComponent<Animator>();
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

        public void SetBuildingBasicEmissionColor(Color basicEmissionColor)
        {
            foreach (MechaComponentModel model in Models)
            {
                model.SetBuildingBasicEmissionColor(basicEmissionColor);
            }
        }

        public void ResetBuildingBasicEmissionColor()
        {
            foreach (MechaComponentModel model in Models)
            {
                model.ResetBuildingBasicEmissionColor();
            }
        }

        public void SetDefaultHighLightEmissionColor(Color highLightEmissionColor)
        {
            foreach (MechaComponentModel model in Models)
            {
                model.SetDefaultHighLightEmissionColor(highLightEmissionColor);
            }
        }

        public void ResetColor()
        {
            foreach (MechaComponentModel model in Models)
            {
                model.ResetColor();
            }
        }

        public void SetAnimTrigger(string trigger)
        {
            if (ModelAnimator) ModelAnimator.SetTrigger(trigger);
        }
    }
}