using System.Collections;
using System.Collections.Generic;
using BiangStudio.GamePlay;
using Client;
using GameCore;
using UnityEditor;
using UnityEngine;

public class MechaComponentScreenShotEditor : ScriptableObject
{
    [MenuItem("Tools/MechaComponentScreenShot")]
    public static void CaptureScreen()
    {
        ConfigManager.LoadAllConfigs();
        if (!PrefabManager.Instance.IsInit)
        {
            PrefabManager.Instance.Awake();
        }

        if (!GameObjectPoolManager.Instance.IsInit)
        {
            GameObjectPoolManager.Instance.Init(new GameObject("GameObjectPoolRoot").transform);
            GameObjectPoolManager.Instance.Awake();
        }

        ClientGameManager.Instance.StartCoroutine(Co_Render());
    }

    static IEnumerator Co_Render()
    {
        Camera camera = CameraManager.Instance.MainCamera;
        float ori_FieldOfView = camera.fieldOfView;
        camera.fieldOfView = 36f;

        foreach (KeyValuePair<string, MechaComponentConfig> kv in ConfigManager.MechaComponentConfigDict)
        {
            MechaComponentInfo mci = new MechaComponentInfo(kv.Value, Quality.Common);
            MechaComponent.BaseInitialize_Editor(mci, null);

            RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 0);

            camera.targetTexture = rt;
            yield return new WaitForSeconds(0.1f);
            camera.Render();

            yield return new WaitForSeconds(0.1f);
            RenderTexture.active = rt;
            Rect rect = new Rect(0, 0, Screen.width, Screen.height);
            Texture2D screenShot = new Texture2D((int) rect.width, (int) rect.height, TextureFormat.RGBA32, false);

            screenShot.ReadPixels(rect, 0, 0);
            screenShot.Apply();

            camera.targetTexture = null;
            RenderTexture.active = null;
            DestroyImmediate(rt);

            yield return new WaitForSeconds(0.1f);
            byte[] bytes = screenShot.EncodeToPNG();
            string filename = Application.dataPath + "/MechaComponentScreenShots/" + kv.Key + ".png";
            System.IO.File.WriteAllBytes(filename, bytes);
            yield return new WaitForSeconds(0.1f);
        }

        camera.fieldOfView = ori_FieldOfView;
    }
}