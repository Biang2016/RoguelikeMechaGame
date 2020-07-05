using BiangStudio.DragHover;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.GamePlay.UI;
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

            caller.Draggable_OnMousePressed(current_DragAreaName);

            float draggedDistance = (buildingMousePos - dragBeginPosition_UIObject).magnitude;
            if (draggedDistance < caller.Draggable_DragMinDistance)
            {
                //不动
            }
            else if (MyDragProcessor.GetCurrentDragAreaName() == DragAreaDefines.Backpack)
            {
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(BackpackManager.Instance.BackpackPanel.ItemContainer.transform as RectTransform, MyDragProcessor.GetDragMousePosition(),
                    UIManager.Instance.UICamera, out Vector2 mousePos))
                {
                    mousePos.x += ((RectTransform) BackpackManager.Instance.BackpackPanel.ItemContainer).rect.width / 2f;
                    mousePos.y -= ((RectTransform) BackpackManager.Instance.BackpackPanel.ItemContainer).rect.height / 2f;
                    int grid_X = Mathf.FloorToInt((mousePos.x) / BackpackManager.Instance.BackpackItemGridSize);
                    int grid_Z = Mathf.FloorToInt((-mousePos.y) / BackpackManager.Instance.BackpackItemGridSize);
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