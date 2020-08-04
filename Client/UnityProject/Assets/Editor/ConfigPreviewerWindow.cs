using System.Collections.Generic;
using GameCore;
using GameCore.AbilityDataDriven;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

public class ConfigPreviewerWindow : OdinEditorWindow
{
    [MenuItem("面板/配置浏览器")]
    public static void OpenConfigPreviewWindow()
    {
        InspectObject(ConfigManager.Instance);
    }
}