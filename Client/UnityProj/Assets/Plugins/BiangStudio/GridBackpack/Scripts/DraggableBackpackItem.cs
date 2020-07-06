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
            RectTransform rectTransform = (RectTransform) BackpackItem.transform;
            RectTransform panelRectTransform = (RectTransform)BackpackItem.Backpack.BackpackPanel.ItemContainer;
            if (wasDragStartThisFrame)
            {
                dragBeginPosition_UIObject = rectTransform.anchoredPosition;
            }

            Vector3 buildingMousePos = MyDragProcessor.GetDragMousePosition();
            caller.Draggable_OnMousePressed(current_DragArea);

            float draggedDistance = (buildingMousePos - dragBeginPosition_UIObject).magnitude;
            if (draggedDistance < caller.Draggable_DragMinDistance)
            {
                //不动
            }
            else if (MyDragProcessor.GetCurrentDragArea().Equals(BackpackItem.Backpack.DragArea))
            {
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    panelRectTransform,
                    buildingMousePos,
                    MyDragProcessor.GetCamera(),
                    out Vector2 anchoredPos))
                {
                    anchoredPos.x += panelRectTransform.rect.width / 2f;
                    anchoredPos.y -= panelRectTransform.rect.height / 2f;
                    int grid_X = Mathf.FloorToInt((anchoredPos.x) / BackpackItem.Backpack.GridSize);
                    int grid_Z = Mathf.FloorToInt((-anchoredPos.y) / BackpackItem.Backpack.GridSize);
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
            transform.localPosition = dragBeginPosition_UIObject;
        }
    }
}