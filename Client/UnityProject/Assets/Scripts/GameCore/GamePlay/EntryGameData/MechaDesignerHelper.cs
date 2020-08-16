using BiangStudio.GameDataFormat.Grid;
using UnityEngine;
using GameCore;
using Sirenix.OdinInspector;

namespace Client
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
                config.GridPosR = GridPosR.GetGridPosByLocalTrans(mcb.transform, 1);
                mechaConfig.MechaComponentList.Add(config);
            }

            return mechaConfig;
        }
    }
}