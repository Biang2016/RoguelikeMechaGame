using Client;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Hierarchy和Project窗口视图扩展
/// </summary>
[InitializeOnLoad]
public class AssetIconCustomDescEditor 
{
    private static AssetIconDescAttribute[] IconTypes = new AssetIconDescAttribute[]
    {
    };
    static AssetIconCustomDescEditor()
    {
        EditorApplication.hierarchyWindowItemOnGUI += DrawIconOnHierarchyWindowItem;
        EditorApplication.projectWindowItemOnGUI += DrawIconOnProjectWindowItem;
    }

    private static void DrawIconOnProjectWindowItem(string guid, Rect rect)
    {
        string assetPath = AssetDatabase.GUIDToAssetPath(guid);
        GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
        if(obj == null)
        {
            return;
        }
        for (int i = 0; i < IconTypes.Length; i++)
        {
            Component cmp = null;
            if(obj.TryGetComponent(IconTypes[i].ClassType, out cmp))
            {
                float iconWidth = 75;
                Vector2 padding = new Vector2(5, 0);
                Rect labelDrawRect = new Rect(rect.xMax - (iconWidth + padding.x), rect.yMin, rect.width, rect.height);
                EditorGUI.LabelField(labelDrawRect, IconTypes[i].Desc);
                break;
            }
        }
    }

    private static void DrawIconOnHierarchyWindowItem(int instanceID, Rect rect)
    {

        GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

        if (obj == null)
        {
            return;
        }

        for (int i = 0; i < IconTypes.Length; i++)
        {
            Component cmp = null;
            if (obj.TryGetComponent(IconTypes[i].ClassType, out cmp))
            {
                float iconWidth = 50;
                Vector2 padding = new Vector2(5, 0);
                Rect labelDrawRect = new Rect(rect.xMax - (iconWidth + padding.x), rect.yMin, rect.width, rect.height);
                EditorGUI.LabelField(labelDrawRect, IconTypes[i].Desc);
                break;
            }
        }

    }
}