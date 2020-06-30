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
        MechaComponentGrids grids = component.MechaComponentGrids;
        GridPos center = grids.GetOccupiedPositions().GetSizeFromListGridPos().center;
        return center.x != 0 || center.z != 0;
    }
}