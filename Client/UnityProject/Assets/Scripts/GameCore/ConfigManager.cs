using System.Collections.Generic;
using System.IO;
using System.Linq;
using BiangStudio;
using BiangStudio.CloneVariant;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.Singleton;
using Client;
using GameCore.AbilityDataDriven;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace GameCore
{
    public class ConfigManager : TSingletonBaseManager<ConfigManager>
    {
        public enum GUID_Separator
        {
            MechaComponentInfo = 2000,
            MechaInfo = 3000,
        }

        public const int GridSize = 1;

        public const int EDIT_AREA_HALF_SIZE = 9;
        public static int EDIT_AREA_FULL_SIZE => EDIT_AREA_HALF_SIZE * 2 + 1;
        public static bool ShowMechaEditorAreaGridPosText = false;

        public const int BackpackGridSize = 60;

        public static string MechaComponentOriginalOccupiedGridInfoJsonFilePath = Application.streamingAssetsPath + "/Configs/MechaComponentPrefabConfigs/MechaComponentOriginalOccupiedGridInfo.json";
        public static Dictionary<string, MechaComponentOriginalOccupiedGridInfo> MechaComponentOriginalOccupiedGridInfoDict = new Dictionary<string, MechaComponentOriginalOccupiedGridInfo>();

        public static string AbilityConfigFolder_Relative = "Configs/BattleConfigs/AbilityConfigs";
        public static string AbilityGroupConfigFolder_Relative = "Configs/BattleConfigs/AbilityGroupConfigs";
        public static string ProjectileConfigFolder_Relative = "Configs/BattleConfigs/ProjectileConfigs";
        public static string MechaComponentGroupConfigFolder_Relative = "Configs/BattleConfigs/MechaComponentGroupConfigs";
        public static string MechaComponentQualityConfigFolder_Relative = "Configs/BattleConfigs/MechaComponentQualityConfigs";
        public static string MechaConfigFolder_Relative = "Configs/MechaConfigs";

        public static string AllMechaComponentConfigPath_Relative = "Configs/BattleConfigs/AllMechaComponentConfig";

        public static string AbilityConfigFolder_Build = Application.streamingAssetsPath + "/" + AbilityConfigFolder_Relative + "/";
        public static string AbilityGroupConfigFolder_Build = Application.streamingAssetsPath + "/" + AbilityGroupConfigFolder_Relative + "/";
        public static string ProjectileConfigFolder_Build = Application.streamingAssetsPath + "/" + ProjectileConfigFolder_Relative + "/";
        public static string MechaComponentGroupConfigFolder_Build = Application.streamingAssetsPath + "/" + MechaComponentGroupConfigFolder_Relative + "/";
        public static string MechaComponentQualityConfigFolder_Build = Application.streamingAssetsPath + "/" + MechaComponentQualityConfigFolder_Relative + "/";
        public static string MechaConfigFolder_Build = Application.streamingAssetsPath + "/" + MechaConfigFolder_Relative + "/";

        public static string AllMechaComponentConfigPath_Build = Application.streamingAssetsPath + "/" + AllMechaComponentConfigPath_Relative + ".config";

        [ShowInInspector]
        [LabelText("技能配置表")]
        public static readonly Dictionary<string, Ability> AbilityConfigDict = new Dictionary<string, Ability>();

        [ShowInInspector]
        [LabelText("技能组配置表")]
        public static readonly Dictionary<string, AbilityGroup> AbilityGroupConfigDict = new Dictionary<string, AbilityGroup>();

        [ShowInInspector]
        [LabelText("投掷物配置表")]
        public static readonly Dictionary<string, ProjectileConfig> ProjectileConfigDict = new Dictionary<string, ProjectileConfig>();

        [ShowInInspector]
        [LabelText("机甲组件配置表")]
        public static readonly Dictionary<string, MechaComponentConfig> MechaComponentConfigDict = new Dictionary<string, MechaComponentConfig>();

        [ShowInInspector]
        [LabelText("机甲组件组配置表")]
        public static readonly Dictionary<string, MechaComponentGroupConfig> MechaComponentGroupConfigDict = new Dictionary<string, MechaComponentGroupConfig>();

        [ShowInInspector]
        [LabelText("机甲组件品质配置表")]
        public static readonly Dictionary<string, MechaComponentQualityConfig> MechaComponentQualityConfigDict = new Dictionary<string, MechaComponentQualityConfig>();

        [ShowInInspector]
        [LabelText("机甲配置表")]
        public static readonly Dictionary<string, MechaConfig> MechaConfigDict = new Dictionary<string, MechaConfig>();

        public override void Awake()
        {
            LoadMechaComponentOccupiedGridPosDict();
            LoadAllConfigs();
        }

        private void LoadMechaComponentOccupiedGridPosDict()
        {
            StreamReader sr = new StreamReader(MechaComponentOriginalOccupiedGridInfoJsonFilePath);
            string content = sr.ReadToEnd();
            sr.Close();
            MechaComponentOriginalOccupiedGridInfoDict = JsonConvert.DeserializeObject<Dictionary<string, MechaComponentOriginalOccupiedGridInfo>>(content);
        }

        [MenuItem("开发工具/配置/序列化配置")]
        public static void ExportConfigs()
        {
            // http://www.sirenix.net/odininspector/faq?Search=&t-11=on#faq

            DataFormat dataFormat = DataFormat.Binary;
            ExportAbilityConfig(dataFormat);
            ExportAbilityGroupConfig(dataFormat);
            ExportProjectileConfig(dataFormat);
            ExportMechaComponentConfig(dataFormat);
            LoadMechaComponentConfig(dataFormat);
            ExportMechaComponentGroupConfig(dataFormat);
            ExportMechaComponentQualityConfig(dataFormat);
            ExportMechaConfig(dataFormat);
            AssetDatabase.Refresh();
        }

        private static void ExportAbilityConfig(DataFormat dataFormat)
        {
            Object[] configObjs = Resources.LoadAll(AbilityConfigFolder_Relative, typeof(Object));
            string folder = AbilityConfigFolder_Build;
            if (Directory.Exists(folder)) Directory.Delete(folder, true);
            Directory.CreateDirectory(folder);
            foreach (Object obj in configObjs)
            {
                AbilityConfigSSO configSSO = (AbilityConfigSSO) obj;
                Ability config = configSSO.Ability;
                string path = folder + configSSO.name + ".config";
                byte[] bytes = SerializationUtility.SerializeValue(config, dataFormat);
                File.WriteAllBytes(path, bytes);
            }
        }

        private static void ExportProjectileConfig(DataFormat dataFormat)
        {
            Object[] configObjs = Resources.LoadAll(ProjectileConfigFolder_Relative, typeof(Object));
            string folder = ProjectileConfigFolder_Build;
            if (Directory.Exists(folder)) Directory.Delete(folder, true);
            Directory.CreateDirectory(folder);
            foreach (Object obj in configObjs)
            {
                ProjectileConfigSSO configSSO = (ProjectileConfigSSO) obj;
                ProjectileConfig config = configSSO.ProjectileConfig;
                string path = folder + configSSO.name + ".config";
                byte[] bytes = SerializationUtility.SerializeValue(config, dataFormat);
                File.WriteAllBytes(path, bytes);
            }
        }

        private static void ExportAbilityGroupConfig(DataFormat dataFormat)
        {
            Object[] configObjs = Resources.LoadAll(AbilityGroupConfigFolder_Relative, typeof(Object));
            string folder = AbilityGroupConfigFolder_Build;
            if (Directory.Exists(folder)) Directory.Delete(folder, true);
            Directory.CreateDirectory(folder);
            foreach (Object obj in configObjs)
            {
                AbilityGroupConfigSSO configSSO = (AbilityGroupConfigSSO) obj;
                AbilityGroup config = configSSO.GetAbilityGroup_NoData();
                string path = folder + configSSO.name + ".config";
                byte[] bytes = SerializationUtility.SerializeValue(config, dataFormat);
                File.WriteAllBytes(path, bytes);
            }
        }

        private static void ExportMechaComponentConfig(DataFormat dataFormat)
        {
            Object configObj = Resources.Load(AllMechaComponentConfigPath_Relative, typeof(Object));
            string path = AllMechaComponentConfigPath_Build;
            FileInfo fi = new FileInfo(path);
            if (fi.Exists) fi.Delete();
            AllMechaComponentConfigSSO configSSO = (AllMechaComponentConfigSSO) configObj;
            configSSO.RefreshConfigList();
            byte[] bytes = SerializationUtility.SerializeValue(configSSO.MechaComponentConfigList, dataFormat);
            File.WriteAllBytes(path, bytes);
        }

        private static void ExportMechaComponentGroupConfig(DataFormat dataFormat)
        {
            Object[] configObjs = Resources.LoadAll(MechaComponentGroupConfigFolder_Relative, typeof(Object));
            string folder = MechaComponentGroupConfigFolder_Build;
            if (Directory.Exists(folder)) Directory.Delete(folder, true);
            Directory.CreateDirectory(folder);
            foreach (Object obj in configObjs)
            {
                MechaComponentGroupConfigSSO configSSO = (MechaComponentGroupConfigSSO) obj;
                configSSO.RefreshConfigListBeforeExport();
                MechaComponentGroupConfig config = configSSO.MechaComponentGroupConfig;
                if (string.IsNullOrEmpty(configSSO.MechaComponentGroupConfig.MechaComponentGroupConfigName))
                {
                    Debug.LogError("机甲组件组配置无名称，无法序列化");
                }
                else
                {
                    string path = folder + configSSO.name + ".config";
                    byte[] bytes = SerializationUtility.SerializeValue(config, dataFormat);
                    File.WriteAllBytes(path, bytes);
                }
            }
        }

        private static void ExportMechaComponentQualityConfig(DataFormat dataFormat)
        {
            Object[] configObjs = Resources.LoadAll(MechaComponentQualityConfigFolder_Relative, typeof(Object));
            string folder = MechaComponentQualityConfigFolder_Build;
            if (Directory.Exists(folder)) Directory.Delete(folder, true);
            Directory.CreateDirectory(folder);
            foreach (Object obj in configObjs)
            {
                MechaComponentQualityConfigSSO configSSO = (MechaComponentQualityConfigSSO) obj;
                MechaComponentQualityConfig config = configSSO.MechaComponentQualityConfig;
                if (string.IsNullOrEmpty(configSSO.MechaComponentQualityConfig.MechaComponentQualityConfigName))
                {
                    Debug.LogError("机甲组件品质配置未绑定机甲组件Prefab，无法序列化");
                }
                else
                {
                    string path = folder + configSSO.name + ".config";
                    byte[] bytes = SerializationUtility.SerializeValue(config, dataFormat);
                    File.WriteAllBytes(path, bytes);
                }
            }
        }

        private static void ExportMechaConfig(DataFormat dataFormat)
        {
            Object[] configObjs = Resources.LoadAll(MechaConfigFolder_Relative, typeof(Object));
            string folder = MechaConfigFolder_Build;
            if (Directory.Exists(folder)) Directory.Delete(folder, true);
            Directory.CreateDirectory(folder);
            foreach (Object obj in configObjs)
            {
                MechaDesignerHelper helper = ((GameObject) obj).GetComponent<MechaDesignerHelper>();
                if (helper)
                {
                    MechaConfig config = helper.ExportMechaConfig();
                    string path = folder + config.MechaConfigName + ".config";
                    byte[] bytes = SerializationUtility.SerializeValue(config, dataFormat);
                    File.WriteAllBytes(path, bytes);
                }
                else
                {
                    Debug.LogError($"{obj.name}机甲配置未绑定脚本MechaDesignerHelper，无法序列化");
                }
            }
        }

        public static bool IsLoaded = false;

        [MenuItem("开发工具/配置/加载配置")]
        public static void LoadAllConfigs()
        {
            DataFormat dataFormat = DataFormat.Binary;
            LoadAbilityConfig(dataFormat);
            LoadAbilityGroupConfig(dataFormat);
            LoadProjectileConfig(dataFormat);
            LoadMechaComponentConfig(dataFormat);
            LoadMechaComponentQualityConfig(dataFormat);
            LoadMechaComponentGroupConfig(dataFormat);
            LoadMechaConfig(dataFormat);
            IsLoaded = true;
        }

        private static void LoadAbilityConfig(DataFormat dataFormat)
        {
            AbilityConfigDict.Clear();

            DirectoryInfo di = new DirectoryInfo(AbilityConfigFolder_Build);
            if (di.Exists)
            {
                foreach (FileInfo fi in di.GetFiles("*.config", SearchOption.AllDirectories))
                {
                    byte[] bytes = File.ReadAllBytes(fi.FullName);
                    Ability config = SerializationUtility.DeserializeValue<Ability>(bytes, dataFormat);
                    if (AbilityConfigDict.ContainsKey(config.AbilityName))
                    {
                        Debug.LogError($"技能重名:{config.AbilityName}");
                    }
                    else
                    {
                        AbilityConfigDict.Add(config.AbilityName, config);
                    }
                }
            }
            else
            {
                Debug.LogError("技能配置表不存在");
            }
        }

        private static void LoadAbilityGroupConfig(DataFormat dataFormat)
        {
            AbilityGroupConfigDict.Clear();
            DirectoryInfo di = new DirectoryInfo(AbilityGroupConfigFolder_Build);
            if (di.Exists)
            {
                foreach (FileInfo fi in di.GetFiles("*.config", SearchOption.AllDirectories))
                {
                    byte[] bytes = File.ReadAllBytes(fi.FullName);
                    AbilityGroup config = SerializationUtility.DeserializeValue<AbilityGroup>(bytes, dataFormat);
                    if (AbilityConfigDict.ContainsKey(config.AbilityGroupName))
                    {
                        Debug.LogError($"技能组重名:{config.AbilityGroupName}");
                    }
                    else
                    {
                        foreach (string ac_name in config.AbilityNames)
                        {
                            if (AbilityConfigDict.TryGetValue(ac_name, out Ability ability))
                            {
                                config.Abilities.Add(ability);
                            }
                        }

                        AbilityGroupConfigDict.Add(config.AbilityGroupName, config);
                    }
                }
            }
            else
            {
                Debug.LogError("技能组配置表不存在");
            }
        }

        private static void LoadProjectileConfig(DataFormat dataFormat)
        {
            ProjectileConfigDict.Clear();

            DirectoryInfo di = new DirectoryInfo(ProjectileConfigFolder_Build);
            if (di.Exists)
            {
                foreach (FileInfo fi in di.GetFiles("*.config", SearchOption.AllDirectories))
                {
                    byte[] bytes = File.ReadAllBytes(fi.FullName);
                    ProjectileConfig config = SerializationUtility.DeserializeValue<ProjectileConfig>(bytes, dataFormat);
                    if (ProjectileConfigDict.ContainsKey(config.ProjectileName))
                    {
                        Debug.LogError($"投掷物重名:{config.ProjectileName}");
                    }
                    else
                    {
                        ProjectileConfigDict.Add(config.ProjectileName, config);
                    }
                }
            }
            else
            {
                Debug.LogError("投掷物表不存在");
            }
        }

        private static void LoadMechaComponentConfig(DataFormat dataFormat)
        {
            MechaComponentConfigDict.Clear();

            FileInfo fi = new FileInfo(AllMechaComponentConfigPath_Build);
            if (fi.Exists)
            {
                byte[] bytes = File.ReadAllBytes(fi.FullName);
                List<MechaComponentConfig> configs = SerializationUtility.DeserializeValue<List<MechaComponentConfig>>(bytes, dataFormat);
                foreach (MechaComponentConfig config in configs)
                {
                    if (MechaComponentConfigDict.ContainsKey(config.MechaComponentKey))
                    {
                        Debug.LogError($"机甲组件配置重名:{config.MechaComponentKey}");
                    }
                    else
                    {
                        MechaComponentConfigDict.Add(config.MechaComponentKey, config);
                    }
                }
            }
            else
            {
                Debug.LogError("机甲组件配置表不存在");
            }
        }

        private static void LoadMechaComponentGroupConfig(DataFormat dataFormat)
        {
            MechaComponentGroupConfigDict.Clear();

            DirectoryInfo di = new DirectoryInfo(MechaComponentGroupConfigFolder_Build);
            if (di.Exists)
            {
                foreach (FileInfo fi in di.GetFiles("*.config", SearchOption.AllDirectories))
                {
                    byte[] bytes = File.ReadAllBytes(fi.FullName);
                    MechaComponentGroupConfig config = SerializationUtility.DeserializeValue<MechaComponentGroupConfig>(bytes, dataFormat);
                    if (MechaComponentGroupConfigDict.ContainsKey(config.MechaComponentGroupConfigName))
                    {
                        Debug.LogError($"机甲组件组配置重名:{config.MechaComponentGroupConfigName}");
                    }
                    else
                    {
                        MechaComponentGroupConfigDict.Add(config.MechaComponentGroupConfigName, config);
                    }
                }
            }
            else
            {
                Debug.LogError("机甲组件组配置表不存在");
            }
        }

        private static void LoadMechaComponentQualityConfig(DataFormat dataFormat)
        {
            MechaComponentQualityConfigDict.Clear();

            DirectoryInfo di = new DirectoryInfo(MechaComponentQualityConfigFolder_Build);
            if (di.Exists)
            {
                foreach (FileInfo fi in di.GetFiles("*.config", SearchOption.AllDirectories))
                {
                    byte[] bytes = File.ReadAllBytes(fi.FullName);
                    MechaComponentQualityConfig config = SerializationUtility.DeserializeValue<MechaComponentQualityConfig>(bytes, dataFormat);
                    if (MechaComponentQualityConfigDict.ContainsKey(config.MechaComponentQualityConfigName))
                    {
                        Debug.LogError($"机甲组件品质配置重名:{config.MechaComponentQualityConfigName}");
                    }
                    else
                    {
                        MechaComponentQualityConfigDict.Add(config.MechaComponentQualityConfigName, config);
                    }
                }
            }
            else
            {
                Debug.LogError("机甲组件品质配置表不存在");
            }
        }

        private static void LoadMechaConfig(DataFormat dataFormat)
        {
            MechaConfigDict.Clear();

            DirectoryInfo di = new DirectoryInfo(MechaConfigFolder_Build);
            if (di.Exists)
            {
                foreach (FileInfo fi in di.GetFiles("*.config", SearchOption.AllDirectories))
                {
                    byte[] bytes = File.ReadAllBytes(fi.FullName);
                    MechaConfig config = SerializationUtility.DeserializeValue<MechaConfig>(bytes, dataFormat);
                    if (MechaConfigDict.ContainsKey(config.MechaConfigName))
                    {
                        Debug.LogError($"机甲配置重名:{config.MechaConfigName}");
                    }
                    else
                    {
                        MechaConfigDict.Add(config.MechaConfigName, config);
                    }
                }
            }
            else
            {
                Debug.LogError("机甲配置表不存在");
            }
        }

        public Ability GetAbility(string abilityName)
        {
            if (!IsLoaded) LoadAllConfigs();
            AbilityConfigDict.TryGetValue(abilityName, out Ability ability);
            return ability?.Clone();
        }

        public AbilityGroup GetAbilityGroup(string abilityGroupName)
        {
            if (!IsLoaded) LoadAllConfigs();
            AbilityGroupConfigDict.TryGetValue(abilityGroupName, out AbilityGroup abilityGroup);
            return abilityGroup?.Clone();
        }

        public ProjectileConfig GetProjectileConfig(string projectileConfigName)
        {
            if (!IsLoaded) LoadAllConfigs();
            ProjectileConfigDict.TryGetValue(projectileConfigName, out ProjectileConfig projectileConfig);
            return projectileConfig?.Clone();
        }

        public MechaComponentConfig GetMechaComponentConfig(string mechaComponentConfigName)
        {
            if (!IsLoaded) LoadAllConfigs();
            MechaComponentConfigDict.TryGetValue(mechaComponentConfigName, out MechaComponentConfig mechaComponentConfig);
            return mechaComponentConfig;
        }

        public MechaComponentGroupConfig GetMechaComponentGroupConfig(string mechaComponentGroupConfigName)
        {
            if (!IsLoaded) LoadAllConfigs();
            MechaComponentGroupConfigDict.TryGetValue(mechaComponentGroupConfigName, out MechaComponentGroupConfig mechaComponentGroupConfig);
            return mechaComponentGroupConfig;
        }

        public MechaComponentQualityConfig GetMechaComponentQualityConfig(string mechaComponentQualityConfigName)
        {
            if (!IsLoaded) LoadAllConfigs();
            MechaComponentQualityConfigDict.TryGetValue(mechaComponentQualityConfigName, out MechaComponentQualityConfig mechaComponentQualityConfig);
            return mechaComponentQualityConfig?.Clone();
        }

        public MechaConfig GetMechaConfig(string mechaConfigName)
        {
            if (!IsLoaded) LoadAllConfigs();
            MechaConfigDict.TryGetValue(mechaConfigName, out MechaConfig mechaConfig);
            return mechaConfig?.Clone();
        }

        public MechaConfig GetRandomMechaConfig()
        {
            if (!IsLoaded) LoadAllConfigs();
            List<MechaConfig> list = CommonUtils.GetRandomFromList(MechaConfigDict.Values.ToList(), 1);
            if (list.Count > 0)
            {
                return list[0].Clone();
            }

            return null;
        }
    }
}