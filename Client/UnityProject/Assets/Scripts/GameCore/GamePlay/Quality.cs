using System.Collections.Generic;
using BiangStudio;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore
{
    public enum Quality
    {
        [LabelText("空")]
        None,

        [LabelText("普通")]
        Common,

        [LabelText("优质")]
        Uncommon,

        [LabelText("稀有")]
        Rare,

        [LabelText("史诗")]
        Epic,

        [LabelText("传说")]
        Legendary,

        [LabelText("神器")]
        Artifact,
    }

    public static class QualityManager
    {
        private static Dictionary<Quality, QualityConfig> QualityConfigs = new Dictionary<Quality, QualityConfig>();

        public static void Initialize()
        {
            QualityConfigs.Clear();
            QualityConfigs.Add(Quality.Common, new QualityConfig(Quality.Common, CommonUtils.HTMLColorToColor("#5CC5CA")));
            QualityConfigs.Add(Quality.Uncommon, new QualityConfig(Quality.Uncommon, CommonUtils.HTMLColorToColor("#108C00")));
            QualityConfigs.Add(Quality.Rare, new QualityConfig(Quality.Rare, CommonUtils.HTMLColorToColor("#0070DD")));
            QualityConfigs.Add(Quality.Epic, new QualityConfig(Quality.Epic, CommonUtils.HTMLColorToColor("#A335EE")));
            QualityConfigs.Add(Quality.Legendary, new QualityConfig(Quality.Legendary, CommonUtils.HTMLColorToColor("#ff8000")));
            QualityConfigs.Add(Quality.Artifact, new QualityConfig(Quality.Artifact, CommonUtils.HTMLColorToColor("#FF3600")));
        }

        public static QualityConfig GetQuality(Quality quality)
        {
            QualityConfigs.TryGetValue(quality, out QualityConfig qualityConfig);
            return qualityConfig;
        }
    }

    public class QualityConfig
    {
        public Quality Quality;
        public Color Color;

        public QualityConfig(Quality quality, Color color)
        {
            Quality = quality;
            Color = color;
        }
    }
}