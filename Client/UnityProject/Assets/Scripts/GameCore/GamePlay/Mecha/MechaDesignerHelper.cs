using BiangStudio.GameDataFormat.Grid;
using UnityEngine;
using GameCore;
using NodeCanvas.BehaviourTrees;

namespace GameCore
{
    public class MechaDesignerHelper : MonoBehaviour
    {
        public MechaConfig ExportMechaConfig()
        {
            MechaConfig mechaConfig = new MechaConfig();
            mechaConfig.MechaConfigName = name;

            MechaComponentBase[] mcs = transform.GetComponentsInChildren<MechaComponentBase>();
            foreach (MechaComponentBase mcb in mcs)
            {
                MechaConfig.Config config = new MechaConfig.Config();
                config.MechaComponentKey = mcb.name;
                config.MechaComponentAlias = mcb.EditorAlias;
                config.MechaComponentQuality = mcb.EditorQuality;
                config.GridPosR = GridPosR.GetGridPosByLocalTrans(mcb.transform, 1);
                mechaConfig.MechaComponentList.Add(config);
            }

            BehaviourTreeOwner bto = GetComponent<BehaviourTreeOwner>();
            if (bto != null)
            {
                mechaConfig.MechaAIConfigKey = bto.behaviour.name;
            }

            return mechaConfig;
        }
    }
}