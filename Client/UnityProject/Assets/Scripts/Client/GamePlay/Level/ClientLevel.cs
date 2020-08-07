using BiangStudio.GamePlay;
using GameCore;
using UnityEngine;

public class ClientLevel : MonoBehaviour
{
    public static ClientLevel GenerateClientLevel(LevelInfo levelInfo)
    {
        GameObject levelPrefab = PrefabManager.Instance.GetPrefab(levelInfo.LevelName);
        GameObject levelGO = Instantiate(levelPrefab);
        ClientLevel clientLevel = levelGO.GetComponent<ClientLevel>();
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