using BiangStudio.Singleton;
using UnityEngine;

namespace Client
{
    public class LayerManager : TSingletonBaseManager<LayerManager>
    {
        public int LayerMask_ComponentHitBox;
        public int LayerMask_BackpackItemHitBox;
        public int LayerMask_DragAreas;
        public int LayerMask_ItemDropped;

        public override void Awake()
        {
            LayerMask_ComponentHitBox = LayerMask.GetMask("ComponentHitBox");
            LayerMask_BackpackItemHitBox = LayerMask.GetMask("BackpackItemHitBox");
            LayerMask_DragAreas = LayerMask.GetMask("DragAreas");
            LayerMask_ItemDropped = LayerMask.GetMask("ItemDropped");
        }
    }
}