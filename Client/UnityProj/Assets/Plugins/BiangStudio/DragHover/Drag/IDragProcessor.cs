using UnityEngine;

namespace BiangStudio.DragHover
{
    public interface IDragProcessor
    {
        Camera GetCamera();
        void ExecuteDrag();
        Vector2 GetDragMousePosition();
        DragArea GetCurrentDragArea();
    }
}