using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;

namespace Client
{
    public class ControlManager : MonoSingleton<ControlManager>
    {
        private PlayerInput PlayerInput;
        public PlayerInput.CommonActions CommonActions;
        public PlayerInput.MechaBattleInputActions MechaBattleInputActions;
        public PlayerInput.MechaBuildingInputActions MechaBuildingInputActions;

        public class ButtonState
        {
            public string ButtonName;
            public bool Down;
            public bool Pressed;
            public bool LastPressed;
            public bool Up;

            public override string ToString()
            {
                if (!Down && !Pressed && !Up) return "";
                string res = ButtonName + (Down ? ",Down" : "") + (Pressed ? ",Pressed" : "") + (Up ? ",Up" : "");
                return res;
            }
        }

        #region Building

        public ButtonState Building_MouseLeft = new ButtonState() {ButtonName = "Building_MouseLeft"};
        public ButtonState Building_MouseRight = new ButtonState() {ButtonName = "Building_MouseRight"};
        public ButtonState Building_MouseMiddle = new ButtonState() {ButtonName = "Building_MouseMiddle"};

        public Vector2 Building_MousePosition;
        public float Building_MouseWheel;

        public ButtonState Building_RotateItem = new ButtonState() {ButtonName = "Building_RotateItem"};
        public ButtonState Building_ToggleBag = new ButtonState() {ButtonName = "Building_ToggleBag"};
        public ButtonState Building_ToggleWireLines = new ButtonState() {ButtonName = "Building_ToggleWireLines"};

        #endregion

        #region Battle

        public ButtonState Battle_MouseLeft = new ButtonState() {ButtonName = "Battle_MouseLeft"};
        public ButtonState Battle_MouseRight = new ButtonState() {ButtonName = "Battle_MouseRight"};
        public ButtonState Battle_MouseMiddle = new ButtonState() {ButtonName = "Battle_MouseMiddle"};
        public ButtonState Battle_Skill_0 = new ButtonState() {ButtonName = "Battle_Skill_0"};
        public ButtonState Battle_Skill_1 = new ButtonState() {ButtonName = "Battle_Skill_1"};
        public ButtonState Battle_Skill_2 = new ButtonState() {ButtonName = "Battle_Skill_2"};
        public ButtonState Battle_Skill_3 = new ButtonState() {ButtonName = "Battle_Skill_3"};

        public Vector2 Battle_Move;
        public Vector2 Battle_MousePosition;
        public float Battle_MouseWheel;

        #endregion

        #region Common

        public ButtonState Common_MouseLeft = new ButtonState() {ButtonName = "Common_MouseLeft"};
        public ButtonState Common_MouseRight = new ButtonState() {ButtonName = "Common_MouseRight"};
        public ButtonState Common_MouseMiddle = new ButtonState() {ButtonName = "Common_MouseMiddle"};

        public Vector2 Common_MousePosition;
        public float Common_MouseWheel;

        public ButtonState Common_Confirm = new ButtonState() {ButtonName = "Common_Confirm"};
        public ButtonState Common_Debug = new ButtonState() {ButtonName = "Common_Debug"};
        public ButtonState Common_Exit = new ButtonState() {ButtonName = "Common_Exit"};
        public ButtonState Common_Tab = new ButtonState() {ButtonName = "Common_Tab"};

        #endregion

        void Awake()
        {
            PlayerInput = new PlayerInput();
            CommonActions = new PlayerInput.CommonActions(PlayerInput);
            MechaBattleInputActions = new PlayerInput.MechaBattleInputActions(PlayerInput);
            MechaBuildingInputActions = new PlayerInput.MechaBuildingInputActions(PlayerInput);

            Building_MouseLeft.GetStateFromContext(MechaBuildingInputActions.MouseLeftClick);
            Building_MouseRight.GetStateFromContext(MechaBuildingInputActions.MouseRightClick);
            Building_MouseMiddle.GetStateFromContext(MechaBuildingInputActions.MouseMiddleClick);

            MechaBuildingInputActions.MousePosition.started += context => Building_MousePosition = context.ReadValue<Vector2>();
            MechaBattleInputActions.MouseWheel.started += context => Building_MouseWheel = context.ReadValue<float>();

            Building_RotateItem.GetStateFromContext(MechaBuildingInputActions.RotateItem);
            Building_ToggleBag.GetStateFromContext(MechaBuildingInputActions.ToggleBag);
            Building_ToggleWireLines.GetStateFromContext(MechaBuildingInputActions.ToggleWireLines);

            Battle_MouseLeft.GetStateFromContext(MechaBattleInputActions.MouseLeftClick);
            Battle_MouseRight.GetStateFromContext(MechaBattleInputActions.MouseRightClick);
            Battle_MouseMiddle.GetStateFromContext(MechaBattleInputActions.MouseMiddleClick);

            MechaBattleInputActions.Move.performed += context => Battle_Move = context.ReadValue<Vector2>();
            MechaBattleInputActions.MousePosition.started += context => Battle_MousePosition = context.ReadValue<Vector2>();
            MechaBattleInputActions.MouseWheel.started += context => Battle_MouseWheel = context.ReadValue<float>();

            Battle_Skill_0.GetStateFromContext(MechaBattleInputActions.Skill_0);
            Battle_Skill_1.GetStateFromContext(MechaBattleInputActions.Skill_1);
            Battle_Skill_2.GetStateFromContext(MechaBattleInputActions.Skill_2);
            Battle_Skill_3.GetStateFromContext(MechaBattleInputActions.Skill_3);

            Common_MouseLeft.GetStateFromContext(CommonActions.MouseLeftClick);
            Common_MouseRight.GetStateFromContext(CommonActions.MouseRightClick);
            Common_MouseMiddle.GetStateFromContext(CommonActions.MouseMiddleClick);

            CommonActions.MousePosition.started += context => Common_MousePosition = context.ReadValue<Vector2>();
            CommonActions.MouseWheel.started += context => Common_MouseWheel = context.ReadValue<float>();

            Common_Confirm.GetStateFromContext(CommonActions.Confirm);
            Common_Debug.GetStateFromContext(CommonActions.Debug);
            Common_Exit.GetStateFromContext(CommonActions.Exit);
            Common_Tab.GetStateFromContext(CommonActions.Tab);
        }

        void Update()
        {
            string input = Common_Exit.ToString();
            if (!string.IsNullOrWhiteSpace(input))
            {
                Debug.Log(Common_Exit.ToString());
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