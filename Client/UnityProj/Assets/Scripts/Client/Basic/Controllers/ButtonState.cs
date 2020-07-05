using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Client
{
    public class ButtonState
    {
        public string ButtonName;
        public bool Down;
        public bool Pressed;
        public bool LastPressed;
        public bool Up;

        public override string ToString()
        {
            if (!Down && !Up) return "";
            string res = ButtonName + (Down ? ",Down" : "") + (Up ? ",Up" : "");
            return res;
        }

        public void Reset()
        {
            Down = false;
            LastPressed = Pressed;
            Up = false;
        }
    }

    public static class ButtonStateHelper
    {
        public static void GetStateCallbackFromContext(this ButtonState state, InputAction action)
        {
            ControlManager.Instance.ButtonStateList.Add(state);
            action.performed += context =>
            {
                ButtonControl bc = (ButtonControl) context.control;
                state.Down = !state.LastPressed;
                state.Pressed = bc.isPressed;
                state.Up = bc.wasReleasedThisFrame;
                if (bc.wasReleasedThisFrame)
                {
                    state.Down = false;
                    state.Pressed = false;
                }
            };

            action.canceled += context => { state.Pressed = false; };
        }
    }
}