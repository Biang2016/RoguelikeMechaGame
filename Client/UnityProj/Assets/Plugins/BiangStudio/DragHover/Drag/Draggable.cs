using UnityEngine;

namespace BiangStudio.DragHover
{
    public abstract class Draggable : MonoBehaviour
    {
        protected IDraggable caller;
        private bool isDragging = false;
        protected bool canDrag;
        protected DragArea dragFrom_DragArea;
        protected DragArea current_DragArea;
        protected bool wasDragStartThisFrame = true;
        protected Vector3 dragBeginPosition_UIObject;
        protected Vector3 oriPosition_WorldObject;
        protected Quaternion oriQuaternion_WorldObject;

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
                if (MyDragProcessor != null)
                {
                    current_DragArea = MyDragProcessor.GetCurrentDragArea();
                }
                else
                {
                    current_DragArea = DragAreaDefines.None;
                }

                OnDragging();
                wasDragStartThisFrame = false;
            }
        }

        protected virtual void OnDragging()
        {
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
                        wasDragStartThisFrame = true;
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
                        caller.Draggable_OnMouseUp(current_DragArea);
                        DragManager.Instance.CurrentDrag = null;
                    }
                    else
                    {
                        DragManager.Instance.CurrentDrag = null;
                    }

                    dragFrom_DragArea = DragAreaDefines.None;
                    current_DragArea = DragAreaDefines.None;
                    wasDragStartThisFrame = false;
                    isDragging = false;
                    MyDragProcessor = null;
                }
            }
        }

        public abstract void ResetToOriginalPositionRotation();
    }
}