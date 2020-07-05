using BiangStudio.DragHover;
using BiangStudio.GameDataFormat.Grid;
using UnityEngine;

namespace BiangStudio.GridBackpack
{
    [RequireComponent(typeof(BackpackItem))]
    [DisallowMultipleComponent]
    public class DraggableBackpackItem : Draggable
    {
        public BackpackItem BackpackItem;

        protected override void OnDragging()
        {
            base.OnDragging();
            Vector3 buildingMousePos = MyDragProcessor.GetDragMousePosition();
            if (wasDragStartThisFrame)
            {
                dragBeginPosition_UIObject = buildingMousePos;
            }

            caller.Draggable_OnMousePressed(current_DragArea);

            float draggedDistance = (buildingMousePos - dragBeginPosition_UIObject).magnitude;
            if (draggedDistance < caller.Draggable_DragMinDistance)
            {
                //不动
            }
            else if (MyDragProcessor.GetCurrentDragArea().Equals(BackpackItem.Backpack.DragArea))
            {
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    BackpackItem.Backpack.BackpackPanel.ItemContainer.transform as RectTransform,
                    MyDragProcessor.GetDragMousePosition(),
                    MyDragProcessor.GetCamera(),
                    out Vector2 mousePos))
                {
                    mousePos.x += ((RectTransform) BackpackItem.Backpack.BackpackPanel.ItemContainer).rect.width / 2f;
                    mousePos.y -= ((RectTransform) BackpackItem.Backpack.BackpackPanel.ItemContainer).rect.height / 2f;
                    int grid_X = Mathf.FloorToInt((mousePos.x) / BackpackItem.Backpack.GridSize);
                    int grid_Z = Mathf.FloorToInt((-mousePos.y) / BackpackItem.Backpack.GridSize);
                    BackpackItem.MoveBaseOnHitBox(new GridPos(grid_X, grid_Z));
                }
            }
            else // drag out of the backpack
            {
                caller.Draggable_DragOutEffects();
            }
        }

        public override void ResetToOriginalPositionRotation()
        {
            transform.localPosition = oriPosition_WorldObject;
            transform.localRotation = oriQuaternion_WorldObject;
        }
    }
}