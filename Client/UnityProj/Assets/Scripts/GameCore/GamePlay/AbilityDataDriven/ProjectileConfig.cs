﻿using BiangStudio.CloneVariant;
using Sirenix.OdinInspector;

namespace GameCore.AbilityDataDriven
{
    public class ProjectileConfig : IClone<ProjectileConfig>
    {
        [LabelText("投掷物名称")]
        public string ProjectileName;


        public ProjectileConfig Clone()
        {
            return new ProjectileConfig
            {
                ProjectileName = ProjectileName,
            };
        }
    }
}