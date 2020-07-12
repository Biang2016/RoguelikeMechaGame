using System.Collections.Generic;
using System.IO;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.Singleton;
using GameCore.AbilityDataDriven;
using Newtonsoft.Json;
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

        public static string BlockOccupiedGridPosJsonFilePath = Application.streamingAssetsPath + "/Configs/BlockConfigs/BlockOccupiedGridPos.json";
        public static SortedDictionary<MechaComponentType, List<GridPos>> MechaComponentOccupiedGridPosDict = new SortedDictionary<MechaComponentType, List<GridPos>>();

        public static string AbilityConfigFolder_Relative = "Configs/BattleConfigs/AbilityConfigs";
        public static string AbilityGroupConfigFolder_Relative = "Configs/BattleConfigs/AbilityGroupConfigs";

        public static string AbilityConfigFolder_Build = Application.streamingAssetsPath + "/" + AbilityConfigFolder_Relative + "/";
        public static string AbilityGroupConfigFolder_Build = Application.streamingAssetsPath + "/" + AbilityGroupConfigFolder_Relative + "/";

        private static Dictionary<string, Ability> AbilityConfigDict = new Dictionary<string, Ability>();
        private static Dictionary<string, AbilityGroup> AbilityGroupConfigDict = new Dictionary<string, AbilityGroup>();

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

        [MenuItem("Tools/序列化技能配置")]
        private static void ExportAbilityConfigs()
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
                    AbilityGroup ag = config.GetAbilityGroup();
                    string path = folder + config.name + ".config";
                    byte[] bytes = SerializationUtility.SerializeValue(ag, dataFormat);
                    File.WriteAllBytes(path, bytes);
                }
            }

            AssetDatabase.Refresh();
        }

        private void LoadAllAbilityConfigs()
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
                        Ability ability = SerializationUtility.DeserializeValue<Ability>(bytes, dataFormat);
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
                        AbilityGroup abilityGroup = SerializationUtility.DeserializeValue<AbilityGroup>(bytes, dataFormat);
                        if (AbilityConfigDict.ContainsKey(abilityGroup.AbilityGroupName))
                        {
                            Debug.LogError($"技能组重名:{abilityGroup.AbilityGroupName}");
                        }
                        else
                        {
                            foreach (string ac_name in abilityGroup.AbilityNames)
                            {
                                if (AbilityConfigDict.TryGetValue(ac_name, out Ability ability))
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
        }

        public Ability GetAbility(string abilityName)
        {
            AbilityConfigDict.TryGetValue(abilityName, out Ability ability);
            return ability?.Clone();
        }

        public AbilityGroup GetAbilityGroup(string abilityGroupName)
        {
            AbilityGroupConfigDict.TryGetValue(abilityGroupName, out AbilityGroup abilityGroup);
            return abilityGroup?.Clone();
        }
    }
}