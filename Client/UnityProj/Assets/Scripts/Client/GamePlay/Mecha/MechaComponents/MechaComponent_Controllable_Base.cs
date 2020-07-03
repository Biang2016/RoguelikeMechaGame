using UnityEngine;
using System.Collections;
using GameCore;

namespace Client
{
    public abstract class MechaComponent_Controllable_Base : MechaComponentBase
    {
        protected override void Update()
        {
            base.Update();
            if (GameStateManager.Instance.GetState() == GameState.Fighting)
            {
                if (ParentMecha && ParentMecha.MechaInfo.MechaType == MechaType.Self)
                {
                    ControlPerFrame();
                }
            }
        }

        protected abstract void ControlPerFrame();
    }
}