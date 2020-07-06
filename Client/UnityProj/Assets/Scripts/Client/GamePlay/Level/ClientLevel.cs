using GameCore;
using UnityEngine;

public class ClientLevel : MonoBehaviour
{
    public Level Level;

    public void Init(Level level)
    {
        Level = level;

        // todo 加载生成场景信息
    }
}
