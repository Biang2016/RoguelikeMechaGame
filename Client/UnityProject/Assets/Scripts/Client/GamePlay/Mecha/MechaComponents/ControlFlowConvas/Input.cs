using System.ComponentModel;
using FlowCanvas.Nodes;
using GameCore;

namespace Client.FlowCanvas
{
    [Category("Input")]
    public class Input : PureFunctionNode<bool, ButtonNames, bool, bool, bool>
    {
        public override bool Invoke(ButtonNames buttonName, bool down, bool up, bool pressed)
        {
            return ControlManager.Instance.CheckButtonAction(buttonName, down, up, pressed);
        }
    }
}