using BiangStudio.GameDataFormat.Grid;
using GameCore;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Client
{
    public static class ClientUtils
    {
        public static GridPos ConvertGridPosToMatrixIndex(this GridPos gp)
        {
            return new GridPos(gp.z + ConfigManager.EDIT_AREA_HALF_SIZE, gp.x + ConfigManager.EDIT_AREA_HALF_SIZE);
        }

        public static GridPos ConvertMatrixIndexToGridPos(this GridPos gp_matrix)
        {
            return new GridPos(gp_matrix.z - ConfigManager.EDIT_AREA_HALF_SIZE, gp_matrix.x - ConfigManager.EDIT_AREA_HALF_SIZE);
        }

        public static void GetStateCallbackFromContext(this ButtonState state, InputAction action)
        {
            ControlManager.Instance.ButtonStateDict.Add(state.ButtonName, state);
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