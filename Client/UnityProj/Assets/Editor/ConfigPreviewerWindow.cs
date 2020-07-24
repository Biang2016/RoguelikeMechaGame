using System.Collections.Generic;
using GameCore;
using GameCore.AbilityDataDriven;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

public class ConfigPreviewerWindow : OdinEditorWindow
{
    [MenuItem("面板/配置浏览器")]
    private static void OpenConfigPreviewWindow()
    {
        EditorUtility.DisplayProgressBar("配置","序列化技能配置",0.5f);

        ConfigManager.ExportAbilityConfigs();

        EditorUtility.DisplayProgressBar("配置","加载技能配置",0.7f);

        ConfigManager.LoadAllAbilityConfigs();

        InspectObject(ConfigManager.Instance);

        EditorUtility.DisplayProgressBar("配置","完毕",1f);
        EditorUtility.ClearProgressBar();
    }
}