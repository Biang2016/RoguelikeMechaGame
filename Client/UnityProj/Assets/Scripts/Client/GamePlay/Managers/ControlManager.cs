using System;
using UnityEngine;
using System.Collections.Generic;
using BiangStudio.Singleton;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

namespace Client
{
    public class ControlManager : TSingletonBaseManager<ControlManager>
    {
        private PlayerInput PlayerInput;
        private PlayerInput.CommonActions CommonInputActions;
        private PlayerInput.MechaBattleInputActions MechaBattleInputActions;
        private PlayerInput.MechaBuildingInputActions MechaBuildingInputActions;

        public List<ButtonState> ButtonStateList = new List<ButtonState>();

        #region Building

        public ButtonState Building_MouseLeft = new ButtonState() {ButtonName = "Building_MouseLeft"};
        public ButtonState Building_MouseRight = new ButtonState() {ButtonName = "Building_MouseRight"};
        public ButtonState Building_MouseMiddle = new ButtonState() {ButtonName = "Building_MouseMiddle"};

        private Vector2 Last_Building_MousePosition = Vector2.zero;

        public Vector2 Building_MousePosition
        {
            get
            {
                if (MechaBuildingInputActions.enabled)
                {
                    Last_Building_MousePosition = MousePosition;
                    return MousePosition;
                }
                else
                {
                    return Last_Building_MousePosition;
                }
            }
        }

        public Vector2 Building_MouseWheel
        {
            get
            {
                if (MechaBuildingInputActions.enabled)
                {
                    return MouseWheel;
                }
                else
                {
                    return Vector2.zero;
                }
            }
        }

        public ButtonState Building_RotateItem = new ButtonState() {ButtonName = "Building_RotateItem"};
        public ButtonState Building_ToggleBackpack = new ButtonState() {ButtonName = "Building_ToggleBackpack"};
        public ButtonState Building_ToggleWireLines = new ButtonState() {ButtonName = "Building_ToggleWireLines"};
        public ButtonState Building_ToggleDebug = new ButtonState() {ButtonName = "Building_ToggleDebug" };

        #endregion

        #region Battle

        public ButtonState Battle_MouseLeft = new ButtonState() {ButtonName = "Battle_MouseLeft"};
        public ButtonState Battle_MouseRight = new ButtonState() {ButtonName = "Battle_MouseRight"};
        public ButtonState Battle_MouseMiddle = new ButtonState() {ButtonName = "Battle_MouseMiddle"};

        public Vector2 Battle_Move;

        private Vector2 Last_Battle_MousePosition = Vector2.zero;

        public Vector2 Battle_MousePosition
        {
            get
            {
                if (MechaBattleInputActions.enabled)
                {
                    Last_Battle_MousePosition = MousePosition;
                    return MousePosition;
                }
                else
                {
                    return Last_Battle_MousePosition;
                }
            }
        }

        public Vector2 Battle_MouseWheel
        {
            get
            {
                if (MechaBattleInputActions.enabled)
                {
                    return MouseWheel;
                }
                else
                {
                    return Vector2.zero;
                }
            }
        }

        public ButtonState Battle_Skill_0 = new ButtonState() {ButtonName = "Battle_Skill_0"};
        public ButtonState Battle_Skill_1 = new ButtonState() {ButtonName = "Battle_Skill_1"};
        public ButtonState Battle_Skill_2 = new ButtonState() {ButtonName = "Battle_Skill_2"};
        public ButtonState Battle_Skill_3 = new ButtonState() {ButtonName = "Battle_Skill_3"};

        #endregion

        #region Common

        public ButtonState Common_MouseLeft = new ButtonState() {ButtonName = "Common_MouseLeft"};
        public ButtonState Common_MouseRight = new ButtonState() {ButtonName = "Common_MouseRight"};
        public ButtonState Common_MouseMiddle = new ButtonState() {ButtonName = "Common_MouseMiddle"};

        private Vector2 Last_Common_MousePosition = Vector2.zero;

        public Vector2 Common_MousePosition
        {
            get
            {
                if (CommonInputActions.enabled)
                {
                    Last_Common_MousePosition = MousePosition;
                    return MousePosition;
                }
                else
                {
                    return Last_Common_MousePosition;
                }
            }
        }

        public Vector2 Common_MouseWheel
        {
            get
            {
                if (CommonInputActions.enabled)
                {
                    return MouseWheel;
                }
                else
                {
                    return Vector2.zero;
                }
            }
        }

        public ButtonState Common_Confirm = new ButtonState() {ButtonName = "Common_Confirm"};
        public ButtonState Common_Debug = new ButtonState() {ButtonName = "Common_Debug"};
        public ButtonState Common_Exit = new ButtonState() {ButtonName = "Common_Exit"};
        public ButtonState Common_Tab = new ButtonState() {ButtonName = "Common_Tab"};

        #endregion

        private Vector2 MousePosition => Mouse.current.position.ReadValue();
        private Vector2 MouseWheel => Mouse.current.scroll.ReadValue();

        public override void Awake()
        {
            PlayerInput = new PlayerInput();
            CommonInputActions = new PlayerInput.CommonActions(PlayerInput);
            MechaBattleInputActions = new PlayerInput.MechaBattleInputActions(PlayerInput);
            MechaBuildingInputActions = new PlayerInput.MechaBuildingInputActions(PlayerInput);

            Building_MouseLeft.GetStateCallbackFromContext(MechaBuildingInputActions.MouseLeftClick);
            Building_MouseRight.GetStateCallbackFromContext(MechaBuildingInputActions.MouseRightClick);
            Building_MouseMiddle.GetStateCallbackFromContext(MechaBuildingInputActions.MouseMiddleClick);

            Building_RotateItem.GetStateCallbackFromContext(MechaBuildingInputActions.RotateItem);
            Building_ToggleBackpack.GetStateCallbackFromContext(MechaBuildingInputActions.ToggleBackpack);
            Building_ToggleWireLines.GetStateCallbackFromContext(MechaBuildingInputActions.ToggleWireLines);
            Building_ToggleDebug.GetStateCallbackFromContext(MechaBuildingInputActions.ToggleDebug);

            Battle_MouseLeft.GetStateCallbackFromContext(MechaBattleInputActions.MouseLeftClick);
            Battle_MouseRight.GetStateCallbackFromContext(MechaBattleInputActions.MouseRightClick);
            Battle_MouseMiddle.GetStateCallbackFromContext(MechaBattleInputActions.MouseMiddleClick);

            MechaBattleInputActions.Move.performed += context => Battle_Move = context.ReadValue<Vector2>();
            MechaBattleInputActions.Move.canceled += context => Battle_Move = Vector2.zero;

            Battle_Skill_0.GetStateCallbackFromContext(MechaBattleInputActions.Skill_0);
            Battle_Skill_1.GetStateCallbackFromContext(MechaBattleInputActions.Skill_1);
            Battle_Skill_2.GetStateCallbackFromContext(MechaBattleInputActions.Skill_2);
            Battle_Skill_3.GetStateCallbackFromContext(MechaBattleInputActions.Skill_3);

            Common_MouseLeft.GetStateCallbackFromContext(CommonInputActions.MouseLeftClick);
            Common_MouseRight.GetStateCallbackFromContext(CommonInputActions.MouseRightClick);
            Common_MouseMiddle.GetStateCallbackFromContext(CommonInputActions.MouseMiddleClick);

            Common_Confirm.GetStateCallbackFromContext(CommonInputActions.Confirm);
            Common_Debug.GetStateCallbackFromContext(CommonInputActions.Debug);
            Common_Exit.GetStateCallbackFromContext(CommonInputActions.Exit);
            Common_Tab.GetStateCallbackFromContext(CommonInputActions.Tab);

            PlayerInput.Enable();
        }

        public override void Update()
        {
            if (false)
            {
                foreach (ButtonState buttonState in ButtonStateList)
                {
                    string input = buttonState.ToString();
                    if (!string.IsNullOrWhiteSpace(input))
                    {
                        Debug.Log(input);
                    }
                }
            }
        }

        public override void LateUpdate()
        {
            foreach (ButtonState buttonState in ButtonStateList)
            {
                buttonState.Reset();
            }
        }

        public void EnableBattleInputActions(bool enable)
        {
            if (enable)
            {
                MechaBattleInputActions.Enable();
            }
            else
            {
                MechaBattleInputActions.Disable();
            }
        }

        public void EnableBuildingInputActions(bool enable)
        {
            if (enable)
            {
                MechaBuildingInputActions.Enable();
            }
            else
            {
                MechaBuildingInputActions.Disable();
            }
        }
    }
}