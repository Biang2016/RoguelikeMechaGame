using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

namespace Client
{
    public class ControlManager : MonoSingleton<ControlManager>
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

        public float Building_MouseWheel;

        public Vector2 Building_MousePosition
        {
            get
            {
                if (MechaBuildingInputActions.enabled)
                {
                    return MousePosition;
                }
                else
                {
                    return Vector2.zero;
                }
            }
        }

        public ButtonState Building_RotateItem = new ButtonState() {ButtonName = "Building_RotateItem"};
        public ButtonState Building_ToggleBag = new ButtonState() {ButtonName = "Building_ToggleBag"};
        public ButtonState Building_ToggleWireLines = new ButtonState() {ButtonName = "Building_ToggleWireLines"};

        #endregion

        #region Battle

        public ButtonState Battle_MouseLeft = new ButtonState() {ButtonName = "Battle_MouseLeft"};
        public ButtonState Battle_MouseRight = new ButtonState() {ButtonName = "Battle_MouseRight"};
        public ButtonState Battle_MouseMiddle = new ButtonState() {ButtonName = "Battle_MouseMiddle"};

        public Vector2 Battle_Move;
        public float Battle_MouseWheel;

        public Vector2 Battle_MousePosition
        {
            get
            {
                if (MechaBattleInputActions.enabled)
                {
                    return MousePosition;
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

        public float Common_MouseWheel;

        public Vector2 Common_MousePosition {
            get
            {
                if (CommonInputActions.enabled)
                {
                    return MousePosition;
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

        public Vector2 MousePosition => Mouse.current.scroll.ReadValue();

        void Awake()
        {
            PlayerInput = new PlayerInput();
            CommonInputActions = new PlayerInput.CommonActions(PlayerInput);
            MechaBattleInputActions = new PlayerInput.MechaBattleInputActions(PlayerInput);
            MechaBuildingInputActions = new PlayerInput.MechaBuildingInputActions(PlayerInput);

            Building_MouseLeft.GetStateCallbackFromContext(MechaBuildingInputActions.MouseLeftClick);
            Building_MouseRight.GetStateCallbackFromContext(MechaBuildingInputActions.MouseRightClick);
            Building_MouseMiddle.GetStateCallbackFromContext(MechaBuildingInputActions.MouseMiddleClick);

            MechaBattleInputActions.MouseWheel.started += context => Building_MouseWheel = context.ReadValue<float>();

            Building_RotateItem.GetStateCallbackFromContext(MechaBuildingInputActions.RotateItem);
            Building_ToggleBag.GetStateCallbackFromContext(MechaBuildingInputActions.ToggleBag);
            Building_ToggleWireLines.GetStateCallbackFromContext(MechaBuildingInputActions.ToggleWireLines);

            Battle_MouseLeft.GetStateCallbackFromContext(MechaBattleInputActions.MouseLeftClick);
            Battle_MouseRight.GetStateCallbackFromContext(MechaBattleInputActions.MouseRightClick);
            Battle_MouseMiddle.GetStateCallbackFromContext(MechaBattleInputActions.MouseMiddleClick);

            MechaBattleInputActions.Move.performed += context => Battle_Move = context.ReadValue<Vector2>();
            MechaBattleInputActions.MouseWheel.started += context => Battle_MouseWheel = context.ReadValue<float>();

            Battle_Skill_0.GetStateCallbackFromContext(MechaBattleInputActions.Skill_0);
            Battle_Skill_1.GetStateCallbackFromContext(MechaBattleInputActions.Skill_1);
            Battle_Skill_2.GetStateCallbackFromContext(MechaBattleInputActions.Skill_2);
            Battle_Skill_3.GetStateCallbackFromContext(MechaBattleInputActions.Skill_3);

            Common_MouseLeft.GetStateCallbackFromContext(CommonInputActions.MouseLeftClick);
            Common_MouseRight.GetStateCallbackFromContext(CommonInputActions.MouseRightClick);
            Common_MouseMiddle.GetStateCallbackFromContext(CommonInputActions.MouseMiddleClick);

            CommonInputActions.MouseWheel.started += context => Common_MouseWheel = context.ReadValue<float>();

            Common_Confirm.GetStateCallbackFromContext(CommonInputActions.Confirm);
            Common_Debug.GetStateCallbackFromContext(CommonInputActions.Debug);
            Common_Exit.GetStateCallbackFromContext(CommonInputActions.Exit);
            Common_Tab.GetStateCallbackFromContext(CommonInputActions.Tab);
        }

        void Update()
        {
            foreach (ButtonState buttonState in ButtonStateList)
            {
                string input = buttonState.ToString();
                if (!string.IsNullOrWhiteSpace(input))
                {
                    Debug.Log(input);
                }
            }

            Debug.Log(Building_MouseWheel);
        }

        void LateUpdate()
        {
            foreach (ButtonState buttonState in ButtonStateList)
            {
                buttonState.Reset();
            }
        }

        void OnEnable()
        {
            PlayerInput.Enable();
        }

        void OnDisable()
        {
            PlayerInput.Disable();
        }
    }
}