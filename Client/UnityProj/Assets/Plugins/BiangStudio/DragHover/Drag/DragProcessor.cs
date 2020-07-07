using UnityEngine;

namespace BiangStudio.DragHover
{
    public abstract class DragProcessor
    {
        public Camera Camera;
        protected int LayerMask;
        protected float MaxRaycastDistance;
        protected DragManager.GetScreenMousePositionDelegate GetScreenMousePositionHandler;
        protected DragManager.ScreenMousePositionToWorldDelegate ScreenMousePositionToWorldHandler;

        public Vector2 DeltaMousePosition_Screen => CurrentMousePosition_Screen - LastMousePosition_Screen;
        public Vector2 LastMousePosition_Screen;
        public Vector2 CurrentMousePosition_Screen;
        public Vector2 DeltaMousePosition_World => CurrentMousePosition_World - LastMousePosition_World;
        public Vector3 LastMousePosition_World;
        public Vector3 CurrentMousePosition_World;

        public void Update()
        {
            LastMousePosition_Screen = CurrentMousePosition_Screen;
            if (GetScreenMousePositionHandler != null)
            {
                CurrentMousePosition_Screen = GetScreenMousePositionHandler.Invoke();
            }
            else
            {
                LastMousePosition_Screen = Vector2.zero;
                CurrentMousePosition_Screen = Vector2.zero;
            }

            LastMousePosition_World = CurrentMousePosition_World;
            if (ScreenMousePositionToWorldHandler != null)
            {
                CurrentMousePosition_World = ScreenMousePositionToWorldHandler.Invoke(CurrentMousePosition_Screen);
            }
            else
            {
                LastMousePosition_World = Vector2.zero;
                CurrentMousePosition_World = Vector2.zero;
            }
        }

        public virtual void ExecuteDrag()
        {
        }

        public void Init(
            Camera camera,
            int layerMask,
            DragManager.GetScreenMousePositionDelegate getScreenMousePositionHandler,
            DragManager.ScreenMousePositionToWorldDelegate screenMousePositionToWorldHandler,
            float maxRaycastDistance = 1000f)
        {
            Camera = camera;
            LayerMask = layerMask;
            GetScreenMousePositionHandler = getScreenMousePositionHandler;
            ScreenMousePositionToWorldHandler = screenMousePositionToWorldHandler;
            MaxRaycastDistance = maxRaycastDistance;
        }

        public DragArea GetCurrentDragArea()
        {
            Ray ray = Camera.ScreenPointToRay(CurrentMousePosition_Screen);
            RaycastHit[] hits = Physics.RaycastAll(ray, 1000f, DragManager.Instance.DragAreaLayerMask);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider)
                {
                    DragAreaIndicator dai = hit.collider.gameObject.GetComponentInParent<DragAreaIndicator>();
                    if (dai)
                    {
                        return dai.DragArea;
                    }
                }
            }

            return DragAreaDefines.None;
        }
    }
}