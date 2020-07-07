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
                if (Mecha && Mecha.MechaInfo.MechaType == MechaType.Player)
                {
                    ControlPerFrame();
                }
            }
        }

        protected abstract void ControlPerFrame();
    }
}