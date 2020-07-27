using System.Collections.Generic;
using System.IO;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.Singleton;
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

        public const int BackpackGridSize = 60;

        public static string BlockOccupiedGridPosJsonFilePath = Application.streamingAssetsPath + "/Configs/BlockConfigs/BlockOccupiedGridPos.json";
        public static SortedDictionary<MechaComponentType, List<GridPos>> MechaComponentOccupiedGridPosDict = new SortedDictionary<MechaComponentType, List<GridPos>>();

        public static string AbilityConfigFolder_Relative = "Configs/BattleConfigs/AbilityConfigs";
        public static string AbilityGroupConfigFolder_Relative = "Configs/BattleConfigs/AbilityGroupConfigs";
        public static string ProjectileConfigFolder_Relative = "Configs/BattleConfigs/ProjectileConfigs";

        public static string AbilityConfigFolder_Build = Application.streamingAssetsPath + "/" + AbilityConfigFolder_Relative + "/";
        public static string AbilityGroupConfigFolder_Build = Application.streamingAssetsPath + "/" + AbilityGroupConfigFolder_Relative + "/";
        public static string ProjectileConfigFolder_Build = Application.streamingAssetsPath + "/" + ProjectileConfigFolder_Relative + "/";

        [ShowInInspector]
        [LabelText("技能配置表")]
        public static readonly Dictionary<string, GamePlayAbility> AbilityConfigDict = new Dictionary<string, GamePlayAbility>();

        [ShowInInspector]
        [LabelText("技能组配置表")]
        public static readonly Dictionary<string, GamePlayAbilityGroup> AbilityGroupConfigDict = new Dictionary<string, GamePlayAbilityGroup>();

        [ShowInInspector]
        [LabelText("投掷物配置表")]
        public static readonly Dictionary<string, ProjectileConfig> ProjectileConfigDict = new Dictionary<string, ProjectileConfig>();

        public override void Awake()
        {
            LoadMechaComponentOccupiedGridPosDict();
            LoadAllAbilityConfigs();
        }

        private void LoadMechaComponentOccupiedGridPosDict()
        {
            StreamReader sr = new StreamReader(BlockOccupiedGridPosJsonFilePath);
            string content = sr.ReadToEnd();
            sr.Close();
            MechaComponentOccupiedGridPosDict = JsonConvert.DeserializeObject<SortedDictionary<MechaComponentType, List<GridPos>>>(content);
        }

        [MenuItem("开发工具/序列化技能配置")]
        public static void ExportAbilityConfigs()
        {
            // http://www.sirenix.net/odininspector/faq?Search=&t-11=on#faq

            DataFormat dataFormat = DataFormat.Binary;
            {
                Object[] configObjs = Resources.LoadAll(AbilityConfigFolder_Relative, typeof(Object));
                string folder = AbilityConfigFolder_Build;
                if (Directory.Exists(folder)) Directory.Delete(folder, true);
                Directory.CreateDirectory(folder);
                foreach (Object obj in configObjs)
                {
                    AbilityConfigSSO config = (AbilityConfigSSO) obj;
                    string path = folder + config.name + ".config";
                    byte[] bytes = SerializationUtility.SerializeValue(config.Ability, dataFormat);
                    File.WriteAllBytes(path, bytes);
                }
            }

            {
                Object[] configObjs = Resources.LoadAll(AbilityGroupConfigFolder_Relative, typeof(Object));
                string folder = AbilityGroupConfigFolder_Build;
                if (Directory.Exists(folder)) Directory.Delete(folder, true);
                Directory.CreateDirectory(folder);
                foreach (Object obj in configObjs)
                {
                    AbilityGroupConfigSSO config = (AbilityGroupConfigSSO) obj;
                    GamePlayAbilityGroup ag = config.GetAbilityGroup_NoData();
                    string path = folder + config.name + ".config";
                    byte[] bytes = SerializationUtility.SerializeValue(ag, dataFormat);
                    File.WriteAllBytes(path, bytes);
                }
            }

            {
                Object[] configObjs = Resources.LoadAll(ProjectileConfigFolder_Relative, typeof(Object));
                string folder = ProjectileConfigFolder_Build;
                if (Directory.Exists(folder)) Directory.Delete(folder, true);
                Directory.CreateDirectory(folder);
                foreach (Object obj in configObjs)
                {
                    ProjectileConfigSSO config = (ProjectileConfigSSO) obj;
                    ProjectileConfig pc = config.ProjectileConfig;
                    string path = folder + config.name + ".config";
                    byte[] bytes = SerializationUtility.SerializeValue(pc, dataFormat);
                    File.WriteAllBytes(path, bytes);
                }
            }

            AssetDatabase.Refresh();
        }

        public static bool IsLoaded = false;

        [MenuItem("开发工具/加载技能配置")]
        public static void LoadAllAbilityConfigs()
        {
            DataFormat dataFormat = DataFormat.Binary;

            {
                AbilityConfigDict.Clear();

                DirectoryInfo di = new DirectoryInfo(AbilityConfigFolder_Build);
                if (di.Exists)
                {
                    foreach (FileInfo fi in di.GetFiles("*.config", SearchOption.AllDirectories))
                    {
                        byte[] bytes = File.ReadAllBytes(fi.FullName);
                        GamePlayAbility ability = SerializationUtility.DeserializeValue<GamePlayAbility>(bytes, dataFormat);
                        if (AbilityConfigDict.ContainsKey(ability.AbilityName))
                        {
                            Debug.LogError($"技能重名:{ability.AbilityName}");
                        }
                        else
                        {
                            AbilityConfigDict.Add(ability.AbilityName, ability);
                        }
                    }
                }
                else
                {
                    Debug.LogError("技能配置表不存在");
                }
            }

            {
                AbilityGroupConfigDict.Clear();
                DirectoryInfo di = new DirectoryInfo(AbilityGroupConfigFolder_Build);
                if (di.Exists)
                {
                    foreach (FileInfo fi in di.GetFiles("*.config", SearchOption.AllDirectories))
                    {
                        byte[] bytes = File.ReadAllBytes(fi.FullName);
                        GamePlayAbilityGroup abilityGroup = SerializationUtility.DeserializeValue<GamePlayAbilityGroup>(bytes, dataFormat);
                        if (AbilityConfigDict.ContainsKey(abilityGroup.AbilityGroupName))
                        {
                            Debug.LogError($"技能组重名:{abilityGroup.AbilityGroupName}");
                        }
                        else
                        {
                            foreach (string ac_name in abilityGroup.AbilityNames)
                            {
                                if (AbilityConfigDict.TryGetValue(ac_name, out GamePlayAbility ability))
                                {
                                    abilityGroup.Abilities.Add(ability);
                                }
                            }

                            AbilityGroupConfigDict.Add(abilityGroup.AbilityGroupName, abilityGroup);
                        }
                    }
                }
                else
                {
                    Debug.LogError("技能组配置表不存在");
                }
            }

            {
                ProjectileConfigDict.Clear();

                DirectoryInfo di = new DirectoryInfo(ProjectileConfigFolder_Build);
                if (di.Exists)
                {
                    foreach (FileInfo fi in di.GetFiles("*.config", SearchOption.AllDirectories))
                    {
                        byte[] bytes = File.ReadAllBytes(fi.FullName);
                        ProjectileConfig pc = SerializationUtility.DeserializeValue<ProjectileConfig>(bytes, dataFormat);
                        if (ProjectileConfigDict.ContainsKey(pc.ProjectileName))
                        {
                            Debug.LogError($"投掷物重名:{pc.ProjectileName}");
                        }
                        else
                        {
                            ProjectileConfigDict.Add(pc.ProjectileName, pc);
                        }
                    }
                }
                else
                {
                    Debug.LogError("投掷物表不存在");
                }
            }
            IsLoaded = true;
        }

        public GamePlayAbility GetAbility(string abilityName)
        {
            if (!IsLoaded) LoadAllAbilityConfigs();
            AbilityConfigDict.TryGetValue(abilityName, out GamePlayAbility ability);
            return ability?.Clone();
        }

        public GamePlayAbilityGroup GetAbilityGroup(string abilityGroupName)
        {
            if (!IsLoaded) LoadAllAbilityConfigs();
            AbilityGroupConfigDict.TryGetValue(abilityGroupName, out GamePlayAbilityGroup abilityGroup);
            return abilityGroup?.Clone();
        }

        public ProjectileConfig GetProjectileConfig(string projectileConfigName)
        {
            if (!IsLoaded) LoadAllAbilityConfigs();
            ProjectileConfigDict.TryGetValue(projectileConfigName, out ProjectileConfig projectileConfig);
            return projectileConfig?.Clone();
        }
    }
}