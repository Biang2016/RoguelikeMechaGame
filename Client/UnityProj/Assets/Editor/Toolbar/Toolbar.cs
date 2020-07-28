﻿using Client;
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
            public static readonly GUIStyle toolbarbuttonLeft;
            public static readonly GUIStyle toolbarbuttonRight;
            public static readonly GUIStyle toolbarbutton;

            static ToolbarStyles()
            {
                toolbarbuttonLeft = new GUIStyle("toolbarbuttonLeft")
                {
                    fontSize = 11,
                    alignment = TextAnchor.MiddleCenter,
                    imagePosition = ImagePosition.ImageAbove,
                    fontStyle = FontStyle.Normal,
                };
                toolbarbuttonRight = new GUIStyle("toolbarbuttonRight")
                {
                    fontSize = 11,
                    alignment = TextAnchor.MiddleCenter,
                    imagePosition = ImagePosition.ImageAbove,
                    fontStyle = FontStyle.Normal,
                };
                toolbarbutton = new GUIStyle("toolbarbutton")
                {
                    fontSize = 11,
                    alignment = TextAnchor.MiddleCenter,
                    imagePosition = ImagePosition.ImageAbove,
                    fontStyle = FontStyle.Normal,
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
            if (GUILayout.Button(new GUIContent("序列化模块占位"), ToolbarStyles.toolbarbutton))
            {
                MechaComponentBase.SerializeMechaComponentOccupiedPositions();
            }

            GUILayout.FlexibleSpace();
        }

        private static void OnRightToolbarGUI()
        {
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(new GUIContent("序列化配置"), ToolbarStyles.toolbarbuttonLeft))
            {
                ConfigManager.ExportConfigs();
                ConfigManager.LoadAllConfigs();
            }

            if (GUILayout.Button(new GUIContent("配置面板"), ToolbarStyles.toolbarbuttonRight))
            {
                ConfigPreviewerWindow.OpenConfigPreviewWindow();
            }
        }
    }
}