using Client;
using GameCore;
using UnityEngine;
using UnityEditor;

namespace UnityToolbarExtender
{
    [InitializeOnLoad]
    public class SceneSwitchLeftButton
    {
        static class ToolbarStyles
        {
            public static readonly GUIStyle commandButtonStyle;

            static ToolbarStyles()
            {
                commandButtonStyle = new GUIStyle("Command")
                {
                    fontSize = 11,
                    alignment = TextAnchor.MiddleCenter,
                    imagePosition = ImagePosition.ImageAbove,
                    fontStyle = FontStyle.Normal,
                    fixedWidth = 120,
                };
            }
        }

        static SceneSwitchLeftButton()
        {
            ToolbarExtender.LeftToolbarGUI.Add(OnLeftToolbarGUI);
            ToolbarExtender.RightToolbarGUI.Add(OnRightToolbarGUI);
        }

        private static void OnLeftToolbarGUI()
        {
            if (GUILayout.Button(new GUIContent("序列化模块占位"), ToolbarStyles.commandButtonStyle))
            {
                MechaComponentBase.SerializeMechaComponentOccupiedPositions();
            }

            GUILayout.FlexibleSpace();
        }

        private static void OnRightToolbarGUI()
        {
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(new GUIContent("序列化技能配置"), ToolbarStyles.commandButtonStyle))
            {
                ConfigManager.ExportAbilityConfigs();
                ConfigManager.LoadAllAbilityConfigs();
            }

            if (GUILayout.Button(new GUIContent("配置面板"), ToolbarStyles.commandButtonStyle))
            {
                ConfigPreviewerWindow.OpenConfigPreviewWindow();
            }
        }
    }
}