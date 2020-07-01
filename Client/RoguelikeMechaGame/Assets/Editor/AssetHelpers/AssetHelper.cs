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
        MechaComponentGridRoot grids = component.MechaComponentGrids;
        GridPos center = grids.GetOccupiedPositions().GetSizeFromListGridPos().center;
        foreach (MechaComponentHitBox hb in component.GetComponentsInChildren<MechaComponentHitBox>())
        {
            hb.transform.Translate(new Vector3(-center.x * GameManager.GridSize, 0, -center.z * GameManager.GridSize));
        }

        foreach (MechaComponentGrid mcg in component.GetComponentsInChildren<MechaComponentGrid>())
        {
            mcg.transform.Translate(new Vector3(-center.x * GameManager.GridSize, 0, -center.z * GameManager.GridSize));
        }

        foreach (MechaComponentModel model in component.GetComponentsInChildren<MechaComponentModel>())
        {
            model.transform.Translate(new Vector3(-center.x * GameManager.GridSize, 0, -center.z * GameManager.GridSize));
        }

        foreach (Shooter st in component.GetComponentsInChildren<Shooter>())
        {
            st.transform.Translate(new Vector3(-center.x * GameManager.GridSize, 0, -center.z * GameManager.GridSize));
        }

        return center.x != 0 || center.z != 0;
    }
}