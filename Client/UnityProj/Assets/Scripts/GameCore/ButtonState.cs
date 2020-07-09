using System;

namespace GameCore
{
    [Serializable]
    public class ButtonState
    {
        public ButtonNames ButtonName;
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

    public enum ButtonNames
    {
        Building_MouseLeft,
        Building_MouseRight,
        Building_MouseMiddle,

        Building_RotateItem,
        Building_ToggleBackpack,
        Building_ToggleWireLines,
        Building_ToggleDebug,

        Battle_MouseLeft,
        Battle_MouseRight,
        Battle_MouseMiddle,

        Battle_Skill_0,
        Battle_Skill_1,
        Battle_Skill_2,
        Battle_Skill_3,

        Common_MouseLeft,
        Common_MouseRight,
        Common_MouseMiddle,

        Common_Confirm,
        Common_Debug,
        Common_Exit,
        Common_Tab
    }
}