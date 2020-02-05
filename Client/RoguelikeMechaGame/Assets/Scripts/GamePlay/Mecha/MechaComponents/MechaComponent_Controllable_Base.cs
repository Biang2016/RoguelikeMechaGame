using UnityEngine;
using System.Collections;

public abstract class MechaComponent_Controllable_Base : MechaComponentBase
{
    void Update()
    {
        if (GameManager.Instance.GetState() == GameState.Fighting)
        {
            if (ParentMecha && ParentMecha.MechaInfo.MechaType == MechaType.Self)
            {
                ControlPerFrame();
            }
        }
    }

    protected abstract void ControlPerFrame();
}