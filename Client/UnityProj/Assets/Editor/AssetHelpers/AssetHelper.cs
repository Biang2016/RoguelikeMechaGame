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
        GridPos center = root.GetOccupiedPositions().GetBoundingRectFromListGridPos().center;
        foreach (MechaComponentHitBox hb in component.MechaComponentGridRoot.HitBoxes)
        {
            hb.transform.Translate(new Vector3(-center.x * ConfigManager.GridSize, 0, -center.z * ConfigManager.GridSize));
        }

        foreach (MechaComponentGrid mcg in component.GetComponentsInChildren<MechaComponentGrid>())
        {
            mcg.transform.Translate(new Vector3(-center.x * ConfigManager.GridSize, 0, -center.z * ConfigManager.GridSize));
        }

        foreach (MechaComponentModel model in component.GetComponentsInChildren<MechaComponentModel>())
        {
            model.transform.Translate(new Vector3(-center.x * ConfigManager.GridSize, 0, -center.z * ConfigManager.GridSize));
        }

        return center.x != 0 || center.z != 0;
    }
}