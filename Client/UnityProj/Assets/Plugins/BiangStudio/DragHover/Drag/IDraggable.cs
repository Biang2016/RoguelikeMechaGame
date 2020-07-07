using UnityEngine;

namespace BiangStudio.DragHover
{
    public interface IDraggable
    {
        /// <summary>
        /// 此接口用于将除了Draggable通用效果之外的效果还给调用者自行处理
        /// </summary>
        /// <param name="collider">点击的碰撞体</param>
        void Draggable_OnMouseDown(DragArea dragAreaName, Collider collider);

        /// <summary>
        /// 传达鼠标左键按住拖动时的鼠标位置信息
        /// </summary>
        /// <param name="dragAreaName">移动到了哪个区域</param>
        /// <param name="diffFromStart">拖拽向量</param>
        void Draggable_OnMousePressed(DragArea dragAreaName, Vector3 diffFromStart);

        /// <summary>
        /// 传达鼠标左键松开时的鼠标位置信息
        /// </summary>
        /// <param name="dragAreaName">移动到了哪个区域</param>
        /// <param name="diffFromStart">拖拽向量</param>
        void Draggable_OnMouseUp(DragArea dragAreaName, Vector3 diffFromStart);

        void Draggable_SetStates(ref bool canDrag, ref DragArea dragFrom);
        float Draggable_DragMinDistance { get; }
        float Draggable_DragMaxDistance { get; }
        void Draggable_DragOutEffects();
    }
}