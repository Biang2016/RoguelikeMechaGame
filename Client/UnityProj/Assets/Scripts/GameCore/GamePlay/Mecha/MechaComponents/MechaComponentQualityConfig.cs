using System;
using System.Collections.Generic;
using BiangStudio.CloneVariant;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore
{
    public class MechaComponentQualityConfig : IClone<MechaComponentQualityConfig>
    {
        [ReadOnly]
        [LabelText("机甲组件品质配置名称")]
        public string MechaComponentQualityConfigName;

        [LabelText("品质能力差异表")]
        [ListDrawerSettings(ListElementLabelName = "Quality")]
        public List<QualityUpgradeDataBase> QualityUpgradeDataList = new List<QualityUpgradeDataBase>();

        public QualityUpgradeDataBase GetQualityUpgradeData(Quality quality)
        {
            foreach (QualityUpgradeDataBase q in QualityUpgradeDataList)
            {
                if (q.Quality == quality)
                {
                    return q;
                }
            }

            return null;
        }

        public MechaComponentQualityConfig Clone()
        {
            MechaComponentQualityConfig newConfig = new MechaComponentQualityConfig();
            newConfig.MechaComponentQualityConfigName = MechaComponentQualityConfigName;
            newConfig.QualityUpgradeDataList = QualityUpgradeDataList.Clone();
            return newConfig;
        }
    }

    public abstract class QualityUpgradeDataBase : IClone<QualityUpgradeDataBase>
    {
        [LabelText("品质")]
        public Quality Quality;

        [LabelText("生命值")]
        public int Life;

        [LabelText("荧光颜色")]
        [ColorPalette("MechaComponentQualityColor")]
        public Color HighLightColor;

        [LabelText("输入功率能力差异表")]
        [ListDrawerSettings(ListElementLabelName = "PowerConsume")]
        [TableList]
        public List<PowerUpgradeDataBase> PowerUpgradeDataList = new List<PowerUpgradeDataBase>();

        [SerializeField]
        public PowerUpgradeDataBase GetPowerUpgradeData(int inputPower)
        {
            SortedDictionary<int, PowerUpgradeDataBase> tempDict = new SortedDictionary<int, PowerUpgradeDataBase>();
            foreach (PowerUpgradeDataBase p in PowerUpgradeDataList)
            {
                tempDict.Add(p.PowerConsume, p);
            }

            PowerUpgradeDataBase lastMatch = null;
            foreach (KeyValuePair<int, PowerUpgradeDataBase> kv in tempDict)
            {
                if (kv.Key > inputPower)
                {
                    return lastMatch;
                }
                else
                {
                    lastMatch = kv.Value;
                }
            }

            return lastMatch;
        }

        public QualityUpgradeDataBase Clone()
        {
            Type type = GetType();
            QualityUpgradeDataBase newConfig = (QualityUpgradeDataBase) Activator.CreateInstance(type);
            newConfig.Quality = Quality;
            newConfig.Life = Life;
            newConfig.HighLightColor = HighLightColor;
            newConfig.PowerUpgradeDataList = PowerUpgradeDataList.Clone();
            ChildClone(newConfig);
            return newConfig;
        }

        protected virtual void ChildClone(QualityUpgradeDataBase newConfig)
        {
        }
    }

    public abstract class PowerUpgradeDataBase : IClone<PowerUpgradeDataBase>
    {
        [TableColumnWidth(10)]
        [VerticalGroup("输入功率阈值(MW)")]
        [HideLabel]
        public int PowerConsume;

        [TableColumnWidth(7)]
        [VerticalGroup("荧光强度")]
        [HideLabel]
        public float HighLightColorIntensity;

        [TableColumnWidth(13)]
        [VerticalGroup("额外获得能力列表")]
        [HideLabel]
        public List<string> AddOnAbilityList = new List<string>();

        public PowerUpgradeDataBase Clone()
        {
            Type type = GetType();
            PowerUpgradeDataBase newConfig = (PowerUpgradeDataBase) Activator.CreateInstance(type);
            newConfig.PowerConsume = PowerConsume;
            newConfig.HighLightColorIntensity = HighLightColorIntensity;
            newConfig.AddOnAbilityList = AddOnAbilityList.Clone();
            ChildClone(newConfig);
            return newConfig;
        }

        protected virtual void ChildClone(PowerUpgradeDataBase newConfig)
        {
        }
    }

    public class QualityUpgradeData_Gun : QualityUpgradeDataBase
    {
    }

    public class PowerUpgradeData_Gun : PowerUpgradeDataBase
    {
        [TableColumnWidth(7)]
        [VerticalGroup("伤害增幅(%)")]
        [HideLabel]
        public int DamageIncreasePercent;

        [TableColumnWidth(8)]
        [VerticalGroup("伤害范围增幅(%)")]
        [HideLabel]
        public int DamageRangeIncreasePercent;

        [TableColumnWidth(12)]
        [VerticalGroup("范围伤害目标上限覆写")]
        [HideLabel]
        public int DamageMaxTargetsOverride;

        [TableColumnWidth(7)]
        [VerticalGroup("冷却时间减免(%)")]
        [HideLabel]
        public int AbilityCooldownDecreasePercent;

        [TableColumnWidth(7)]
        [VerticalGroup("射程增幅(%)")]
        [HideLabel]
        public int MaxRangeIncreasePercent;

        [TableColumnWidth(10)]
        [VerticalGroup("生存最大时间增幅(%)")]
        [HideLabel]
        public int MaxDurationIncreasePercent;

        [TableColumnWidth(8)]
        [VerticalGroup("初始尺寸增幅(%)")]
        [HideLabel]
        public int ScaleIncreasePercent;

        [TableColumnWidth(8)]
        [VerticalGroup("轴向速度增幅(%)")]
        [HideLabel]
        public int VelocityIncreasePercent;

        [TableColumnWidth(8)]
        [VerticalGroup("反弹能力覆写")]
        [HideLabel]
        public bool CanReflectOverride;

        [TableColumnWidth(10)]
        [VerticalGroup("最大反弹次数覆写")]
        [HideLabel]
        public int ReflectTimesOverride;

        protected override void ChildClone(PowerUpgradeDataBase newConfig)
        {
            base.ChildClone(newConfig);
            PowerUpgradeData_Gun config = ((PowerUpgradeData_Gun) newConfig);
            config.DamageIncreasePercent = DamageIncreasePercent;
            config.DamageRangeIncreasePercent = DamageRangeIncreasePercent;
            config.DamageMaxTargetsOverride = DamageMaxTargetsOverride;
            config.AbilityCooldownDecreasePercent = AbilityCooldownDecreasePercent;
            config.MaxRangeIncreasePercent = MaxRangeIncreasePercent;
            config.MaxDurationIncreasePercent = MaxDurationIncreasePercent;
            config.ScaleIncreasePercent = ScaleIncreasePercent;
            config.VelocityIncreasePercent = VelocityIncreasePercent;
            config.CanReflectOverride = CanReflectOverride;
            config.ReflectTimesOverride = ReflectTimesOverride;
        }
    }

    public class QualityUpgradeData_Core : QualityUpgradeDataBase
    {
    }

    public class PowerUpgradeData_Core : PowerUpgradeDataBase
    {
        [TableColumnWidth(8)]
        [VerticalGroup("冷却时间减免(%)")]
        [HideLabel]
        public int AbilityCooldownDecreasePercent;

        protected override void ChildClone(PowerUpgradeDataBase newConfig)
        {
            base.ChildClone(newConfig);
            PowerUpgradeData_Core config = ((PowerUpgradeData_Core) newConfig);
            config.AbilityCooldownDecreasePercent = AbilityCooldownDecreasePercent;
        }
    }
}