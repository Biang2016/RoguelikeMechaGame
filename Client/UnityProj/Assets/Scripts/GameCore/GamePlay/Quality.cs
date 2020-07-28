using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    public enum Quality
    {
        Poor,
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
        Artifact,
    }

    public static class QualityManager
    {
        private static Dictionary<Quality, QualityConfig> QualityConfigs = new Dictionary<Quality, QualityConfig>();

        public static void Initialize()
        {
            QualityConfigs.Add(Quality.Poor, new QualityConfig(Quality.Poor, Utils.HTMLColorToColor("#9d9d9d")));
            QualityConfigs.Add(Quality.Common, new QualityConfig(Quality.Poor, Utils.HTMLColorToColor("#ffffff")));
            QualityConfigs.Add(Quality.Uncommon, new QualityConfig(Quality.Poor, Utils.HTMLColorToColor("#1eff00")));
            QualityConfigs.Add(Quality.Rare, new QualityConfig(Quality.Poor, Utils.HTMLColorToColor("#0070dd")));
            QualityConfigs.Add(Quality.Epic, new QualityConfig(Quality.Poor, Utils.HTMLColorToColor("#a335ee")));
            QualityConfigs.Add(Quality.Legendary, new QualityConfig(Quality.Poor, Utils.HTMLColorToColor("#ff8000")));
            QualityConfigs.Add(Quality.Artifact, new QualityConfig(Quality.Poor, Utils.HTMLColorToColor("#e6cc80")));
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