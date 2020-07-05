﻿using BiangStudio.DragHover;
using UnityEngine;

namespace Client
{
    [RequireComponent(typeof(MechaComponentBase))]
    [DisallowMultipleComponent]
    public class DraggableMechaComponent : Draggable
    {
        protected override void OnDragging()
        {
            base.OnDragging();

            if (wasDragStartThisFrame)
            {
                oriPosition_WorldObject = transform.localPosition;
                oriQuaternion_WorldObject = transform.localRotation;
            }

            caller.Draggable_OnMousePressed(current_DragAreaName);
        }

        public override void ResetToOriginalPositionRotation()
        {
            transform.localPosition = oriPosition_WorldObject;
            transform.localRotation = oriQuaternion_WorldObject;
        }
    }
}