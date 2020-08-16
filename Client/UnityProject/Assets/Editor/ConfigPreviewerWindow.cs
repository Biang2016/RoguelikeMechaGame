using System.Collections.Generic;
using System.Linq;
using GameCore;
using GameCore.AbilityDataDriven;
using NodeCanvas.BehaviourTrees;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

public class ConfigPreviewerWindow : OdinEditorWindow
{
    [MenuItem("开发工具/面板/配置浏览器")]
    public static void OpenConfigPreviewWindow()
    {
        ConfigManager.LoadAllConfigs();
        GetWindow<ConfigPreviewerWindow>().Show();
    }

    [ShowInInspector]
    [LabelText("技能配置表")]
    [TableList]
    public List<Ability> AbilityConfigDict => ConfigManager.AbilityConfigDict.Values.ToList();

    [ShowInInspector]
    [LabelText("技能组配置表")]
    [TableList]
    public List<AbilityGroup> AbilityGroupConfigDict => ConfigManager.AbilityGroupConfigDict.Values.ToList();

    [ShowInInspector]
    [LabelText("投掷物配置表")]
    [TableList]
    public List<ProjectileConfig> ProjectileConfigDict => ConfigManager.ProjectileConfigDict.Values.ToList();

    [ShowInInspector]
    [LabelText("机甲组件配置表")]
    [TableList]
    public List<MechaComponentConfig> MechaComponentConfigDict => ConfigManager.MechaComponentConfigDict.Values.ToList();

    [ShowInInspector]
    [Title("机甲组件组配置表")]
    public Dictionary<string, MechaComponentGroupConfig> MechaComponentGroupConfigDict => ConfigManager.MechaComponentGroupConfigDict;

    [ShowInInspector]
    [Title("机甲组件品质配置表")]
    public Dictionary<string, MechaComponentQualityConfig> MechaComponentQualityConfigDict => ConfigManager.MechaComponentQualityConfigDict;

    [ShowInInspector]
    [Title("机甲配置表")]
    public Dictionary<string, MechaConfig> MechaConfigDict => ConfigManager.MechaConfigDict;

    [ShowInInspector]
    [Title("敌兵AI配置表")]
    public Dictionary<string, BehaviourTree> EnemyAIConfigDict => ConfigManager.EnemyAIConfigDict;
}