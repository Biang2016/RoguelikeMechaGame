using UnityEngine;

namespace BiangStudio.DragHover
{
    public interface IDragProcessor
    {
        void ExecuteDrag();
        Vector2 GetDragMousePosition();
        string GetCurrentDragAreaName();
    }
}