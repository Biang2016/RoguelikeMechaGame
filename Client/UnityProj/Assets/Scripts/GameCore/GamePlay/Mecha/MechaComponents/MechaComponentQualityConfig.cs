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

        [LabelText("输入功率能力差异表")]
        [ListDrawerSettings(ListElementLabelName = "PowerConsume")]
        public List<PowerUpgradeDataBase> PowerUpgradeDataList = new List<PowerUpgradeDataBase>();

        [SerializeField]
        public PowerUpgradeDataBase GetPowerUpgradeData(int inputPower)
        {
            SortedDictionary<int, PowerUpgradeDataBase> tempDict = new SortedDictionary<int, PowerUpgradeDataBase>();
            foreach (PowerUpgradeDataBase p in PowerUpgradeDataList)
            {
                tempDict.Add(p.PowerConsume, p);
            }

            PowerUpgradeDataBase last = null;
            foreach (KeyValuePair<int, PowerUpgradeDataBase> kv in tempDict)
            {
                if (kv.Key > inputPower)
                {
                    return last;
                }

                last = kv.Value;
            }

            return null;
        }

        public QualityUpgradeDataBase Clone()
        {
            Type type = GetType();
            QualityUpgradeDataBase newConfig = (QualityUpgradeDataBase) Activator.CreateInstance(type);
            newConfig.Quality = Quality;
            newConfig.Life = Life;
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
        [LabelText("输入功率阈值(MW)")]
        public int PowerConsume;

        [LabelText("额外获得能力列表")]
        public List<string> AddOnAbilityList = new List<string>();

        public PowerUpgradeDataBase Clone()
        {
            Type type = GetType();
            PowerUpgradeDataBase newConfig = (PowerUpgradeDataBase) Activator.CreateInstance(type);
            newConfig.PowerConsume = PowerConsume;
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
        [LabelText("伤害增幅(%)")]
        public int DamageIncreasePercent;

        [LabelText("冷却时间减免(%)")]
        public int AbilityCooldownDecreasePercent;

        [LabelText("射程增幅(%)")]
        public int MaxRangeIncreasePercent;

        [LabelText("生存最大时间增幅(%)")]
        public int MaxDurationIncreasePercent;

        [LabelText("初始尺寸增幅(%)")]
        public int ScaleIncreasePercent;

        [LabelText("轴向速度增幅(%)")]
        public int VelocityIncreasePercent;

        [LabelText("反弹能力覆写")]
        public bool CanReflectOverride;

        [LabelText("最大反弹次数覆写")]
        public int ReflectTimesOverride;

        protected override void ChildClone(PowerUpgradeDataBase newConfig)
        {
            base.ChildClone(newConfig);
            PowerUpgradeData_Gun config = ((PowerUpgradeData_Gun) newConfig);
            config.DamageIncreasePercent = DamageIncreasePercent;
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
        [LabelText("冷却时间减免(%)")]
        public int AbilityCooldownDecreasePercent;

        protected override void ChildClone(PowerUpgradeDataBase newConfig)
        {
            base.ChildClone(newConfig);
            PowerUpgradeData_Core config = ((PowerUpgradeData_Core) newConfig);
            config.AbilityCooldownDecreasePercent = AbilityCooldownDecreasePercent;
        }
    }
}