using System.Linq;
using BiangStudio.GameDataFormat.Grid;
using Client;
using GameCore;
using UnityEditor.SceneManagement;
using UnityEngine;

public class AssetHelper : UnityEditor.AssetModificationProcessor
{
    public static string[] OnWillSaveAssets(string[] paths)
    {
        foreach (string path in paths)
        {
            if (path.Contains("MechaComponent_"))
            {
                MechaComponentBase[] components = StageUtility.GetCurrentStageHandle().FindComponentsOfType<MechaComponentBase>();
                foreach (MechaComponentBase component in components)
                {
                    ProcessMechaComponentPrefab(component);
                }
            }
        }

        return paths;
    }

    private static bool ProcessMechaComponentPrefab(MechaComponentBase component)
    {
        component.transform.position = Vector3.zero;
        component.transform.rotation = Quaternion.identity;
        MechaComponentGridRoot root = component.MechaComponentGridRoot;
        root.RefreshConfig();
        GridPos center = root.GetOccupiedPositions().GetBoundingRectFromListGridPos().center;
        foreach (MechaComponentGrid grid in root.mechaComponentGrids)
        {
            grid.transform.Translate(new Vector3(-center.x * ConfigManager.GridSize, 0, -center.z * ConfigManager.GridSize));
        }

        foreach (MechaComponentModel model in component.GetComponentsInChildren<MechaComponentModel>())
        {
            model.transform.Translate(new Vector3(-center.x * ConfigManager.GridSize, 0, -center.z * ConfigManager.GridSize));
        }

        return center.x != 0 || center.z != 0;
    }
}