using UnityEngine;

namespace BiangStudio.DragHover
{
    public abstract class Draggable : MonoBehaviour
    {
        protected IDraggable caller;
        private bool isDragging = false;
        protected bool canDrag;
        protected DragArea dragFrom_DragArea;
        protected Vector3 oriPosition;
        protected Quaternion oriQuaternion;

        public IDragProcessor MyDragProcessor;

        void Awake()
        {
            caller = GetComponent<IDraggable>();
            if (caller == null)
            {
                DragManager.LogError("Couldn't find IDraggable in the gameObject with Draggable component.");
            }
        }

        void Update()
        {
            if (!canDrag) return;
            if (isDragging)
            {
                caller.Draggable_OnMousePressed(DragManager.Instance.Current_DragArea);
            }
        }

        public void SetOnDrag(bool drag, Collider collider, IDragProcessor dragProcessor)
        {
            if (isDragging != drag)
            {
                if (drag) // Press
                {
                    MyDragProcessor = dragProcessor;
                    caller.Draggable_SetStates(ref canDrag, ref dragFrom_DragArea);
                    if (canDrag)
                    {
                        caller.Draggable_OnMouseDown(dragFrom_DragArea, collider);
                        isDragging = true;
                    }
                    else
                    {
                        isDragging = false;
                        DragManager.Instance.CancelDrag();
                    }
                }
                else // Release
                {
                    if (canDrag)
                    {
                        caller.Draggable_OnMouseUp(DragManager.Instance.Current_DragArea);
                        DragManager.Instance.CurrentDrag = null;
                    }
                    else
                    {
                        DragManager.Instance.CurrentDrag = null;
                    }

                    dragFrom_DragArea = DragAreaDefines.None;
                    isDragging = false;
                    MyDragProcessor = null;
                }
            }
        }

        public abstract void ResetToOriginalPositionRotation();
    }
}