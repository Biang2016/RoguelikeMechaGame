using GameCore;
using UnityEngine;

public class ClientLevel : MonoBehaviour
{
    public static ClientLevel GenerateClientLevel(LevelInfo levelInfo)
    {
        GameObject levelGO = new GameObject($"Level_{levelInfo.LevelName}");
        ClientLevel clientLevel = levelGO.AddComponent<ClientLevel>();
        clientLevel.Init(levelInfo);
        return clientLevel;
    }

    public LevelInfo LevelInfo;

    public void Init(LevelInfo levelInfo)
    {
        LevelInfo = levelInfo;

        // todo 加载生成场景信息
    }
}