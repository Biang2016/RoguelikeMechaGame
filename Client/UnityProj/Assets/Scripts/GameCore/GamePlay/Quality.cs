using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore
{
    public enum Quality
    {
        [LabelText("空")]
        None,

        [LabelText("破旧")]
        Poor,

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
            QualityConfigs.Add(Quality.Poor, new QualityConfig(Quality.Poor, Utils.HTMLColorToColor("#9d9d9d")));
            QualityConfigs.Add(Quality.Common, new QualityConfig(Quality.Common, Utils.HTMLColorToColor("#ffffff")));
            QualityConfigs.Add(Quality.Uncommon, new QualityConfig(Quality.Uncommon, Utils.HTMLColorToColor("#1eff00")));
            QualityConfigs.Add(Quality.Rare, new QualityConfig(Quality.Rare, Utils.HTMLColorToColor("#0070dd")));
            QualityConfigs.Add(Quality.Epic, new QualityConfig(Quality.Epic, Utils.HTMLColorToColor("#a335ee")));
            QualityConfigs.Add(Quality.Legendary, new QualityConfig(Quality.Legendary, Utils.HTMLColorToColor("#ff8000")));
            QualityConfigs.Add(Quality.Artifact, new QualityConfig(Quality.Artifact, Utils.HTMLColorToColor("#e6cc80")));
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