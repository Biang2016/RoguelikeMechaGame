﻿using UnityEngine;
using UnityEngine.Events;

namespace BiangStudio.DragHover
{
    public class DragProcessor<T> : IDragProcessor where T : MonoBehaviour
    {
        private Camera Camera;
        private int LayerMask;
        private float MaxRaycastDistance;
        private DragManager.MousePositionDelegate MousePositionHandler;
        private UnityAction<T, Collider, IDragProcessor> OnBeginDrag;
        private UnityAction<T, Collider, IDragProcessor> OnCancelDrag;

        internal DragManager.DragAreaDelegate DragAreaDelegate;
        internal DragManager.MousePositionDelegate DragMousePositionDelegate;

        public bool ExecuteThisFrame = false;

        private Vector2 currentMousePosition
        {
            get
            {
                if (MousePositionHandler != null)
                {
                    return MousePositionHandler.Invoke();
                }
                else
                {
                    return Vector2.zero;
                }
            }
        }

        public void Init(Camera camera, int layerMask, DragManager.MousePositionDelegate mousePositionHandler, UnityAction<T, Collider, IDragProcessor> onBeginDrag, UnityAction<T, Collider, IDragProcessor> onCancelDrag, float maxRaycastDistance = 1000f)
        {
            Camera = camera;
            LayerMask = layerMask;
            MousePositionHandler = mousePositionHandler;
            OnBeginDrag = onBeginDrag;
            OnCancelDrag = onCancelDrag;
            MaxRaycastDistance = maxRaycastDistance;
            DragAreaDelegate = GetCurrentDragAreaName;
            DragMousePositionDelegate = GetDragMousePosition;
            DragManager.Instance.RegisterDragProcessor(this);
        }

        public void ExecuteDrag()
        {
            Ray ray = Camera.ScreenPointToRay(currentMousePosition);
            Physics.Raycast(ray, out RaycastHit hit, MaxRaycastDistance, LayerMask);
            if (hit.collider)
            {
                T dragItem = hit.collider.gameObject.GetComponentInParent<T>();
                if (dragItem)
                {
                    DragManager.Instance.OnCancelDrag = () => OnCancelDrag?.Invoke(dragItem, hit.collider, this);
                    DragManager.Instance.CurrentDrag = dragItem.gameObject.GetComponent<Draggable>();
                    DragManager.Instance.CurrentDrag.SetOnDrag(true, hit.collider, this);
                    OnBeginDrag?.Invoke(dragItem, hit.collider, this);
                }
            }
        }

        public Vector2 GetDragMousePosition()
        {
            return currentMousePosition;
        }

        public string GetCurrentDragAreaName()
        {
            Ray ray = Camera.ScreenPointToRay(currentMousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, 1000f, DragManager.Instance.DragAreaLayerMask);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider)
                {
                    DragArea da = hit.collider.gameObject.GetComponentInParent<DragArea>();
                    if (da)
                    {
                        return da.DragAreaName;
                    }
                }
            }

            return DragAreaDefines.None;
        }
    }
}