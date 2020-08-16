using BiangStudio.ObjectPool;
using Sirenix.OdinInspector;

namespace GameCore
{
    public abstract class MechaComponentBase : PoolObject
    {
        [HideInPlayMode]
        [TitleGroup(("Editor Params"))]
        [LabelText("[机甲编辑器] 组件花名")]
        public string EditorAlias;

        [HideInPlayMode]
        [TitleGroup("Editor Params")]
        [LabelText("[机甲编辑器] 组件品质")]
        public Quality EditorQuality;
    }
}