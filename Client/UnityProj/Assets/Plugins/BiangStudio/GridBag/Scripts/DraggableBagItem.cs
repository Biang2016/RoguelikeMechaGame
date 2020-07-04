using BiangStudio.DragHover;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.GamePlay.UI;
using UnityEngine;

namespace BiangStudio.GridBag
{
    public class DraggableBagItem : Draggable
    {
        public BagItem BagItem;

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
            else if (MyDragProcessor.GetCurrentDragAreaName() == DragAreaDefines.Bag) 
            {
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(BagManager.Instance.BagPanel.ItemContainer.transform as RectTransform, MyDragProcessor.GetDragMousePosition(),
                    UIManager.Instance.UICamera, out Vector2 mousePos))
                {
                    mousePos.x += ((RectTransform)BagManager.Instance.BagPanel.ItemContainer).rect.width / 2f;
                    mousePos.y -= ((RectTransform)BagManager.Instance.BagPanel.ItemContainer).rect.height / 2f;
                    int grid_X = Mathf.FloorToInt((mousePos.x) / BagManager.Instance.BagItemGridSize);
                    int grid_Z = Mathf.FloorToInt((-mousePos.y) / BagManager.Instance.BagItemGridSize);
                    BagItem.MoveBaseOnHitBox(new GridPos(grid_X, grid_Z));
                }
            }
            else // drag out of the bag
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