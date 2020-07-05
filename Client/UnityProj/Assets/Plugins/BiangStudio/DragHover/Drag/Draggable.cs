using UnityEngine;

namespace BiangStudio.DragHover
{
    public abstract class Draggable : MonoBehaviour
    {
        protected IDraggable caller;
        private bool isDragging = false;
        protected bool canDrag;
        protected string dragFrom_DragAreaName;
        protected string current_DragAreaName;
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
                    current_DragAreaName = MyDragProcessor.GetCurrentDragAreaName();
                }
                else
                {
                    current_DragAreaName = DragAreaDefines.None;
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
                    caller.Draggable_SetStates(ref canDrag, ref dragFrom_DragAreaName);
                    if (canDrag)
                    {
                        wasDragStartThisFrame = true;
                        caller.Draggable_OnMouseDown(dragFrom_DragAreaName, collider);
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
                        caller.Draggable_OnMouseUp(current_DragAreaName);
                        DragManager.Instance.CurrentDrag = null;
                    }
                    else
                    {
                        DragManager.Instance.CurrentDrag = null;
                    }

                    dragFrom_DragAreaName = DragAreaDefines.None;
                    current_DragAreaName = DragAreaDefines.None;
                    wasDragStartThisFrame = false;
                    isDragging = false;
                    MyDragProcessor = null;
                }
            }
        }

        public abstract void ResetToOriginalPositionRotation();
    }
}