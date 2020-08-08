using System.Collections;
using System.Collections.Generic;
using System.IO;
using BiangStudio.GameDataFormat.Grid;
using Client;
using GameCore;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public class MechaComponentScreenShotEditor : Editor
{
    [MenuItem("开发工具/机甲组件拍照工具")]
    public static void CaptureScreen()
    {
        if (!Application.isPlaying)
        {
            EditorUtility.DisplayDialog("提示", "需要在运行时使用", "好嘞");
            return;
        }

        ClientGameManager.Instance.StartCoroutine(Co_Render());
    }

    static IEnumerator Co_Render()
    {
        ClientLevelManager.Instance.CurrentClientLevel.gameObject.SetActive(false);
        ClientBattleManager.Instance.MechaContainerRoot.gameObject.SetActive(false);
        Camera camera = CameraManager.Instance.MainCamera;
        Quaternion oriRot = camera.transform.rotation;
        Quaternion topDownViewRot = Quaternion.Euler(90, 0, 0);
        float ori_FieldOfView = camera.fieldOfView;
        camera.fieldOfView = 6f;
        CameraFollow.NeedLerp = false;

        foreach (KeyValuePair<string, MechaComponentConfig> kv in ConfigManager.MechaComponentConfigDict)
        {
            string filename = Application.dataPath + "/../../Arts/MechaComponentScreenShots/" + kv.Key + ".png";
            MechaComponentInfo mci = new MechaComponentInfo(kv.Value, Quality.Common);
            GridRect boundingRect = mci.IInventoryItemContentInfo_OriginalOccupiedGridPositions.GetBoundingRectFromListGridPos();
            if (boundingRect.size.x != boundingRect.size.z)
            {
                camera.transform.rotation = topDownViewRot;
            }
            else
            {
                camera.transform.rotation = oriRot;
            }

            if (boundingRect.size.x == 1 && boundingRect.size.z == 1)
            {
                camera.fieldOfView = 6f;
            }
            else
            {
                camera.fieldOfView = 10f;
            }

            yield return new WaitForSeconds(0.1f);
            MechaComponent mc = MechaComponent.BaseInitialize_Editor(mci, null);
            CaptureScreenShot.CaptureTransparentScreenShot(camera, 800, 800, filename);
            DestroyImmediate(mc.gameObject);
            yield return new WaitForSeconds(0.1f);
        }

        camera.transform.rotation = oriRot;
        camera.fieldOfView = ori_FieldOfView;
        CameraFollow.NeedLerp = true;
        ClientLevelManager.Instance.CurrentClientLevel.gameObject.SetActive(true);
        ClientBattleManager.Instance.MechaContainerRoot.gameObject.SetActive(true);
    }
}

public static class CaptureScreenShot
{
    public static void CaptureTransparentScreenShot(Camera cam, int width, int height, string screenGrabFile_Path)
    {
        // This is slower, but seems more reliable.
        RenderTexture bak_cam_targetTexture = cam.targetTexture;
        CameraClearFlags bak_cam_clearFlags = cam.clearFlags;
        RenderTexture bak_RenderTexture_active = RenderTexture.active;

        Texture2D tex_white = new Texture2D(width, height, TextureFormat.ARGB32, false);
        Texture2D tex_black = new Texture2D(width, height, TextureFormat.ARGB32, false);
        Texture2D tex_transparent = new Texture2D(width, height, TextureFormat.ARGB32, false);
        // Must use 24-bit depth buffer to be able to fill background.
        RenderTexture render_texture = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
        Rect grab_area = new Rect(0, 0, width, height);

        RenderTexture.active = render_texture;
        cam.targetTexture = render_texture;
        cam.clearFlags = CameraClearFlags.SolidColor;

        cam.backgroundColor = Color.black;
        cam.Render();
        tex_black.ReadPixels(grab_area, 0, 0);
        tex_black.Apply();

        cam.backgroundColor = Color.white;
        cam.Render();
        tex_white.ReadPixels(grab_area, 0, 0);
        tex_white.Apply();

        // Create Alpha from the difference between black and white camera renders
        for (int y = 0; y < tex_transparent.height; ++y)
        {
            for (int x = 0; x < tex_transparent.width; ++x)
            {
                float alpha = tex_white.GetPixel(x, y).r - tex_black.GetPixel(x, y).r;
                alpha = 1.0f - alpha;
                Color color;
                if (alpha.Equals(0))
                {
                    color = Color.clear;
                }
                else
                {
                    color = tex_black.GetPixel(x, y) / alpha;
                }

                color.a = alpha;
                tex_transparent.SetPixel(x, y, color);
            }
        }

        // Encode the resulting output texture to a byte array then write to the file
        Rect croppedRect = CropRect(tex_transparent);
        Texture2D cropTexture = new Texture2D((int) croppedRect.width, (int) croppedRect.height, TextureFormat.ARGB32, false);
        Color[] cropPixels = tex_transparent.GetPixels((int) croppedRect.x, (int) croppedRect.y, (int) croppedRect.width, (int) croppedRect.height);
        cropTexture.SetPixels(cropPixels);
        byte[] pngShot = ImageConversion.EncodeToPNG(cropTexture);
        File.WriteAllBytes(screenGrabFile_Path, pngShot);

        cam.clearFlags = bak_cam_clearFlags;
        cam.targetTexture = bak_cam_targetTexture;
        RenderTexture.active = bak_RenderTexture_active;
        RenderTexture.ReleaseTemporary(render_texture);

        Object.Destroy(tex_black);
        Object.Destroy(tex_white);
        Object.Destroy(tex_transparent);
        Object.Destroy(cropTexture);
    }

    public static void SimpleCaptureTransparentScreenShot(Camera cam, int width, int height, string screenGrabFile_Path)
    {
        // Depending on your render pipeline, this may not work.
        RenderTexture bak_cam_targetTexture = cam.targetTexture;
        CameraClearFlags bak_cam_clearFlags = cam.clearFlags;
        RenderTexture bak_RenderTexture_active = RenderTexture.active;

        Texture2D tex_transparent = new Texture2D(width, height, TextureFormat.ARGB32, false);
        // Must use 24-bit depth buffer to be able to fill background.
        RenderTexture render_texture = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
        Rect grab_area = new Rect(0, 0, width, height);

        RenderTexture.active = render_texture;
        cam.targetTexture = render_texture;
        cam.clearFlags = CameraClearFlags.SolidColor;

        // Simple: use a clear background
        cam.backgroundColor = Color.clear;
        cam.Render();
        tex_transparent.ReadPixels(grab_area, 0, 0);
        tex_transparent.Apply();

        // Encode the resulting output texture to a byte array then write to the file
        Rect croppedRect = CropRect(tex_transparent);
        Texture2D tex_transparent_cropped = tex_transparent.CropTexture(croppedRect);
        byte[] pngShot = ImageConversion.EncodeToPNG(tex_transparent_cropped);
        File.WriteAllBytes(screenGrabFile_Path, pngShot);

        cam.clearFlags = bak_cam_clearFlags;
        cam.targetTexture = bak_cam_targetTexture;
        RenderTexture.active = bak_RenderTexture_active;
        RenderTexture.ReleaseTemporary(render_texture);

        Object.Destroy(tex_transparent);
        Object.Destroy(tex_transparent_cropped);
    }

    private static Rect CropRect(Texture2D texture)
    {
        Color[] pixels = texture.GetPixels();
        int newLeft = -1;
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                int index = CoordinateToGridIndex(x, y, texture.width, texture.height);
                Color color = pixels[index];
                if (color.a > 0.01f)
                {
                    newLeft = x;
                    break;
                }
            }

            if (newLeft != -1)
            {
                break;
            }
        }

        int newRight = -1;
        for (int x = texture.width - 1; x >= 0; x--)
        {
            for (int y = 0; y < texture.height; y++)
            {
                int index = CoordinateToGridIndex(x, y, texture.width, texture.height);
                Color color = pixels[index];
                if (color.a > 0.01f)
                {
                    newRight = x;
                    break;
                }
            }

            if (newRight != -1)
            {
                break;
            }
        }

        int newTop = -1;
        for (int y = texture.height - 1; y >= 0; y--)
        {
            for (int x = 0; x < texture.width; x++)
            {
                int index = CoordinateToGridIndex(x, y, texture.width, texture.height);
                Color color = pixels[index];
                if (color.a > 0.01f)
                {
                    newTop = y;
                    break;
                }
            }

            if (newTop != -1)
            {
                break;
            }
        }

        int newBottom = -1;
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                int index = CoordinateToGridIndex(x, y, texture.width, texture.height);
                Color color = pixels[index];
                if (color.a > 0.01f)
                {
                    newBottom = y;
                    break;
                }
            }

            if (newBottom != -1)
            {
                break;
            }
        }

        return new Rect(newLeft, newBottom, newRight - newLeft, newTop - newBottom);
    }

    private static int CoordinateToGridIndex(int x, int y, int xMax, int yMax)
    {
        if (x >= xMax || y >= yMax || x < 0 || y < 0)
        {
            return -1;
        }

        return ((y * xMax) + x);
    }
}