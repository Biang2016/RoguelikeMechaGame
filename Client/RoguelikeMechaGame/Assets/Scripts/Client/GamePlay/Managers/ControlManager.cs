using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Client
{
    public class ControlManager : MonoSingleton<ControlManager>
    {
        private PlayerInput PlayerInput;
        public PlayerInput.CommonActions CommonActions;
        public PlayerInput.MechaBattleInputActions MechaBattleInputActions;
        public PlayerInput.MechaBuildingInputActions MechaBuildingInputActions;

        public struct ButtonState
        {
            public bool Down;
            public bool Pressed;
            public bool Up;
        }

        #region Building

        public ButtonState Building_MouseLeft;
        public ButtonState Building_MouseRight;
        public ButtonState Building_MouseMiddle;

        public Vector2 Building_MousePosition;
        public float Building_MouseWheel;

        public ButtonState Building_RotateItem;
        public ButtonState Building_ToggleBag;
        public ButtonState Building_ToggleWireLines;

        #endregion

        #region Battle

        public ButtonState Battle_MouseLeft;
        public ButtonState Battle_MouseRight;
        public ButtonState Battle_MouseMiddle;
        public ButtonState Battle_Skill_0;
        public ButtonState Battle_Skill_1;
        public ButtonState Battle_Skill_2;
        public ButtonState Battle_Skill_3;

        public Vector2 Battle_Move;
        public Vector2 Battle_MousePosition;
        public float Battle_MouseWheel;

        #endregion

        #region Common

        public ButtonState Common_MouseLeft;
        public ButtonState Common_MouseRight;
        public ButtonState Common_MouseMiddle;

        public Vector2 Common_MousePosition;
        public float Common_MouseWheel;

        public ButtonState Common_Confirm;
        public ButtonState Common_Debug;
        public ButtonState Common_Exit;
        public ButtonState Common_Tab;

        #endregion

        void Awake()
        {
            PlayerInput = new PlayerInput();
            CommonActions = new PlayerInput.CommonActions(PlayerInput);
            MechaBattleInputActions = new PlayerInput.MechaBattleInputActions(PlayerInput);
            MechaBuildingInputActions = new PlayerInput.MechaBuildingInputActions(PlayerInput);

            MechaBuildingInputActions.MouseLeftClick.performed += context => Building_MouseLeft.GetStateFromContext(context);
            MechaBuildingInputActions.MouseRightClick.performed += context => Building_MouseRight.GetStateFromContext(context);
            MechaBuildingInputActions.MouseMiddleClick.performed += context => Building_MouseMiddle.GetStateFromContext(context);

            MechaBuildingInputActions.MousePosition.performed += context => { Building_MousePosition = context.ReadValue<Vector2>(); };
            MechaBattleInputActions.MouseWheel.performed += context => { Building_MouseWheel = context.ReadValue<float>(); };

            MechaBuildingInputActions.RotateItem.performed += context => Building_RotateItem.GetStateFromContext(context);
            MechaBuildingInputActions.ToggleBag.performed += context => Building_ToggleBag.GetStateFromContext(context);
            MechaBuildingInputActions.ToggleWireLines.performed += context => Building_ToggleWireLines.GetStateFromContext(context);

            MechaBattleInputActions.MouseLeftClick.performed += context => Battle_MouseLeft.GetStateFromContext(context);
            MechaBattleInputActions.MouseRightClick.performed += context => Battle_MouseRight.GetStateFromContext(context);
            MechaBattleInputActions.MouseMiddleClick.performed += context => Battle_MouseMiddle.GetStateFromContext(context);

            MechaBattleInputActions.Move.performed += context => { Battle_Move = context.ReadValue<Vector2>(); };
            MechaBattleInputActions.MousePosition.performed += context => { Battle_MousePosition = context.ReadValue<Vector2>(); };
            MechaBattleInputActions.MouseWheel.performed += context => { Battle_MouseWheel = context.ReadValue<float>(); };

            MechaBattleInputActions.Skill_0.performed += context => Battle_Skill_0.GetStateFromContext(context);
            MechaBattleInputActions.Skill_1.performed += context => Battle_Skill_1.GetStateFromContext(context);
            MechaBattleInputActions.Skill_2.performed += context => Battle_Skill_2.GetStateFromContext(context);
            MechaBattleInputActions.Skill_3.performed += context => Battle_Skill_3.GetStateFromContext(context);

            CommonActions.MouseLeftClick.performed += context => Common_MouseLeft.GetStateFromContext(context);
            CommonActions.MouseRightClick.performed += context => Common_MouseRight.GetStateFromContext(context);
            CommonActions.MouseMiddleClick.performed += context => Common_MouseMiddle.GetStateFromContext(context);

            CommonActions.MousePosition.performed += context => { Common_MousePosition = context.ReadValue<Vector2>(); };
            CommonActions.MouseWheel.performed += context => { Common_MouseWheel = context.ReadValue<float>(); };
            CommonActions.Confirm.performed += context => Common_Confirm.GetStateFromContext(context);
            CommonActions.Debug.performed += context => Common_Debug.GetStateFromContext(context);
            CommonActions.Exit.performed += context => Common_Exit.GetStateFromContext(context);
            CommonActions.Tab.performed += context => Common_Tab.GetStateFromContext(context);
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