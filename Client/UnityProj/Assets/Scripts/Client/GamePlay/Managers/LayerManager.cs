using BiangStudio.Singleton;
using UnityEngine;

namespace Client
{
    public class LayerManager : TSingletonBaseManager<LayerManager>
    {
        public int LayerMask_ComponentHitBox;
        public int LayerMask_BagItemHitBox;
        public int LayerMask_DragAreas;
        public int LayerMask_ItemDropped;

        public override void Awake()
        {
            LayerMask_ComponentHitBox = LayerMask.GetMask("ComponentHitBox");
            LayerMask_BagItemHitBox = LayerMask.GetMask("BagItemHitBox");
            LayerMask_DragAreas = LayerMask.GetMask("DragAreas");
            LayerMask_ItemDropped = LayerMask.GetMask("ItemDropped");
        }
    }
}