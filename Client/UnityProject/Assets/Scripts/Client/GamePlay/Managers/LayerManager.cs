﻿using BiangStudio.Singleton;
using UnityEngine;

namespace Client
{
    public class LayerManager : TSingletonBaseManager<LayerManager>
    {
        public int LayerMask_UI;
        public int LayerMask_ComponentHitBox;
        public int LayerMask_BackpackItemGrid;
        public int LayerMask_DragAreas;
        public int LayerMask_ItemDropped;

        public override void Awake()
        {
            LayerMask_UI = LayerMask.GetMask("UI");
            LayerMask_ComponentHitBox = LayerMask.GetMask("ComponentHitBox");
            LayerMask_BackpackItemGrid = LayerMask.GetMask("BackpackItemGrid");
            LayerMask_DragAreas = LayerMask.GetMask("DragAreas");
            LayerMask_ItemDropped = LayerMask.GetMask("ItemDropped");
        }
    }
}