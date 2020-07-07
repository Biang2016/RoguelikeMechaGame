using System.Collections.Generic;
using BiangStudio.Singleton;
using UnityEngine;
using UnityEngine.Events;

namespace BiangStudio.DragHover
{
    public class DragManager : TSingletonBaseManager<DragManager>
    {
        public delegate void LogErrorDelegate(string log);

        public delegate bool DragKeyDelegate();

        public delegate Vector2 GetScreenMousePositionDelegate();
        public delegate Vector3 ScreenMousePositionToWorldDelegate(Vector2 mousePos);

        private LogErrorDelegate LogErrorHandler;
        private DragKeyDelegate DragKeyDownHandler;
        private DragKeyDelegate DragKeyUpHandler;
        internal int DragAreaLayerMask;

        internal UnityAction OnCancelDrag;
        private List<DragProcessor> DragProcessors = new List<DragProcessor>();

        private Draggable currentDrag;
        public bool ForbidDrag = false;
        public DragArea Current_DragArea;

        public Draggable CurrentDrag
        {
            get { return currentDrag; }
            set
            {
                if (currentDrag != value)
                {
                    CancelDrag();
                    currentDrag = value;
                }
            }
        }

        public void Init(DragKeyDelegate dragKeyDownHandler, DragKeyDelegate dragKeyUpHandler, LogErrorDelegate logErrorHandler, int dragAreaLayerMask)
        {
            DragKeyDownHandler = dragKeyDownHandler;
            DragKeyUpHandler = dragKeyUpHandler;
            LogErrorHandler = logErrorHandler;
            DragAreaLayerMask = dragAreaLayerMask;
        }

        public override void Update()
        {
            Current_DragArea = CheckCurrentDragArea();
            if (ForbidDrag)
            {
                CancelDrag();
            }
            else
            {
                CheckDragStartOrEnd();
            }
        }

        internal void RegisterDragProcessor(DragProcessor dragProcessor)
        {
            if (!DragProcessors.Contains(dragProcessor))
            {
                DragProcessors.Add(dragProcessor);
            }
        }

        internal void UnregisterDragProcessor(DragProcessor dragProcessor)
        {
            DragProcessors.Remove(dragProcessor);
        }

        private void CheckDragStartOrEnd()
        {
            if (DragKeyDownHandler != null && DragKeyDownHandler.Invoke())
            {
                foreach (DragProcessor dragProcessor in DragProcessors)
                {
                    dragProcessor.Update();
                    if (!CurrentDrag)
                    {
                        dragProcessor.ExecuteDrag();
                    }
                }
            }

            if (DragKeyUpHandler != null && DragKeyUpHandler.Invoke())
            {
                CancelDrag();
            }
        }

        private DragArea CheckCurrentDragArea()
        {
            foreach (DragProcessor dragProcessor in DragProcessors)
            {
                DragArea dragArea = dragProcessor.GetCurrentDragArea();
                if (!dragArea.Equals(DragAreaDefines.None))
                {
                    return dragArea;
                }
            }

            return DragAreaDefines.None;
        }

        internal void CancelDrag()
        {
            if (currentDrag)
            {
                OnCancelDrag?.Invoke();
                Draggable cDrag = currentDrag;
                currentDrag = null;
                cDrag.SetOnDrag(false, null, null);
            }
        }

        public DragProcessor<T> GetDragProcessor<T>() where T : MonoBehaviour
        {
            foreach (DragProcessor dragProcessor in DragProcessors)
            {
                if (dragProcessor is DragProcessor<T> dp)
                {
                    return dp;
                }
            }

            return null;
        }

        internal static void LogError(string log)
        {
            Instance.LogErrorHandler?.Invoke(log);
        }
    }
}