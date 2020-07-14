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
        ConfigManager.LoadAllAbilityConfigs();
        InspectObject(ConfigManager.Instance);
    }

    //[ShowInInspector]
    //private static Dictionary<string, Ability> AbilityConfigDict => ConfigManager.AbilityConfigDict;

    //[ShowInInspector]
    //private static Dictionary<string, AbilityGroup> AbilityGroupConfigDict => ConfigManager.AbilityGroupConfigDict;

    //[ShowInInspector]
    //private static Dictionary<string, ProjectileConfig> ProjectileConfigDict => ConfigManager.ProjectileConfigDict;
}