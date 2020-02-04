using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可拖动组件，应用于机甲部件、背包物品
/// </summary>
public class Draggable : MonoBehaviour
{
    private IDraggable caller;

    void Awake()
    {
        caller = GetComponent<IDraggable>();
    }

    private bool canDrag;
    private DragAreaTypes dragFrom;

    private bool isBegin = true;

    private Vector3 dragBeginPosition_WorldObject;
    private Vector3 dragBeginPosition_UIObject;

    private Vector3 oriPosition_WorldObject;
    private Quaternion oriQuaternion_WorldObject;
    private Vector2 oriAnchoredPosition_UIObject;

    private Vector3 mOffset;

    void Update()
    {
        if (!canDrag) return;
        if (IsOnDrag)
        {
            Vector3 uiCameraPosition = UIManager.Instance.UICamera.ScreenToWorldPoint(Input.mousePosition);

            if (isBegin)
            {
                mOffset = gameObject.transform.position - GetMouseAsWorldPoint();
            }

            switch (dragFrom)
            {
                case DragAreaTypes.MechaEditorArea:
                {
                    if (isBegin)
                    {
                        dragBeginPosition_WorldObject = GetMouseAsWorldPoint() + mOffset + new Vector3(0.5f, 0, 0.5f) * GameManager.GridSize;
                        oriPosition_WorldObject = transform.localPosition;
                        oriQuaternion_WorldObject = transform.localRotation;
                    }

                    caller.DragComponent_OnMousePressed(CheckMoveToArea()); //将鼠标悬停的区域告知拖动对象主体

                    float draggedDistance = (transform.position - dragBeginPosition_WorldObject).magnitude;
                    if (draggedDistance < caller.DragComponent_DragMinDistance) //不动
                    {
                    }
                    else if (draggedDistance < caller.DragComponent_DragMaxDistance) //拖拽物体本身 
                    {
                        Vector3 newPos = GetMouseAsWorldPoint() + mOffset + new Vector3(0.5f, 0, 0.5f) * GameManager.GridSize;
                        transform.position = new Vector3(newPos.x, transform.position.y, newPos.z);
                    }
                    else //消失 （显示特效或其他逻辑）
                    {
                        caller.DragComponent_DragOutEffects();
                    }

                    break;
                }
                case DragAreaTypes.Bag:
                {
                    if (isBegin)
                    {
                        dragBeginPosition_UIObject = uiCameraPosition;
                        oriAnchoredPosition_UIObject = ((RectTransform) transform).anchoredPosition;
                    }

                    caller.DragComponent_OnMousePressed(CheckMoveToArea()); //将鼠标悬停的区域告知拖动对象主体

                    float draggedDistance = (uiCameraPosition - dragBeginPosition_UIObject).magnitude;
                    if (draggedDistance < caller.DragComponent_DragMinDistance)
                    {
                        //不动
                    }
                    else if (DragManager.Instance.IsMouseInsideBag) //拖拽物体本身 
                    {
                        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(BagManager.Instance.BagPanel.ItemContainer.transform as RectTransform, Input.mousePosition, UIManager.Instance.UICamera, out Vector2 mousePos))
                        {
                            mousePos.x += ((RectTransform) BagManager.Instance.BagPanel.ItemContainer).rect.width / 2f;
                            mousePos.y -= ((RectTransform) BagManager.Instance.BagPanel.ItemContainer).rect.height / 2f;
                            int grid_X = Mathf.FloorToInt((mousePos.x) / BagManager.Instance.BagItemGridSize);
                            int grid_Y = Mathf.FloorToInt((-mousePos.y) / BagManager.Instance.BagItemGridSize);

                            int grid_X_delta = grid_X - DragManager.Instance.CurrentDrag_BagItem.GridPos.x;
                            int grid_Y_delta = grid_Y - DragManager.Instance.CurrentDrag_BagItem.GridPos.z;

                            if (grid_X_delta != 0)
                            {
                                bool suc = BagManager.Instance.BagPanel.CheckSpaceLocked(DragManager.Instance.CurrentDrag_BagItem.RealPositionsInBagPanel, new GridPos(grid_X_delta, 0));
                                if (suc)
                                {
                                    int x = grid_X * BagManager.Instance.BagItemGridSize;
                                    ((RectTransform) transform).anchoredPosition = new Vector2(x, ((RectTransform) transform).anchoredPosition.y);
                                }
                            }

                            if (grid_Y_delta != 0)
                            {
                                bool suc = BagManager.Instance.BagPanel.CheckSpaceLocked(DragManager.Instance.CurrentDrag_BagItem.RealPositionsInBagPanel, new GridPos(0, grid_Y_delta));
                                if (suc)
                                {
                                    int y = grid_Y * BagManager.Instance.BagItemGridSize;
                                    ((RectTransform) transform).anchoredPosition = new Vector2(((RectTransform) transform).anchoredPosition.x, -y);
                                }
                            }
                        }
                    }
                    else //拖出背包
                    {
                        caller.DragComponent_DragOutEffects();
                    }

                    break;
                }
            }

            isBegin = false;
        }
    }

    private bool _isOnDrag = false;

    public bool IsOnDrag
    {
        get { return _isOnDrag; }

        set
        {
            if (_isOnDrag != value)
            {
                if (value) //鼠标按下
                {
                    caller.DragComponent_SetStates(ref canDrag, ref dragFrom);
                    if (canDrag)
                    {
                        caller.DragComponent_OnMouseDown();
                        _isOnDrag = true;
                    }
                    else
                    {
                        _isOnDrag = false;
                        DragManager.Instance.CancelCurrentDrag();
                    }
                }
                else //鼠标放开
                {
                    if (canDrag)
                    {
                        caller.DragComponent_OnMouseUp(CheckMoveToArea()); //将鼠标放开的区域告知拖动对象主体，并提供拖动起始姿态信息以供还原
                        isBegin = true;
                        DragManager.Instance.CurrentDrag = null;
                    }
                    else
                    {
                        DragManager.Instance.CurrentDrag = null;
                    }

                    _isOnDrag = false;
                }
            }
        }
    }

    public void ReturnOriginalPositionRotation()
    {
        transform.localPosition = oriPosition_WorldObject;
        transform.localRotation = oriQuaternion_WorldObject;
    }

    public Vector3 GetMouseAsWorldPoint()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = GameManager.Instance.MainCamera.WorldToScreenPoint(gameObject.transform.position).z;
        return GameManager.Instance.MainCamera.ScreenToWorldPoint(mousePoint);
    }

    public DragAreaTypes CheckMoveToArea()
    {
        if (DragManager.Instance.IsMouseInsideBag) return DragAreaTypes.Bag;
        Ray ray = GameManager.Instance.MainCamera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit raycast, 200f, GameManager.Instance.LayerMask_DragAreas);
        if (raycast.collider)
        {
            DragArea da = raycast.collider.gameObject.GetComponent<DragArea>();
            if (da)
            {
                return da.DragAreaTypes;
            }
        }

        return DragAreaTypes.None;
    }
}

internal interface IDraggable
{
    /// <summary>
    /// 此接口用于将除了Draggable通用效果之外的效果还给调用者自行处理
    /// </summary>
    void DragComponent_OnMouseDown();

    /// <summary>
    /// 传达鼠标左键按住拖动时的鼠标位置信息
    /// </summary>
    void DragComponent_OnMousePressed(DragAreaTypes dragAreaTypes);

    /// <summary>
    /// 传达鼠标左键松开时的鼠标位置信息
    /// </summary>
    /// <param name="dragAreaTypes">移动到了哪个区域</param>
    void DragComponent_OnMouseUp(DragAreaTypes dragAreaTypes);

    void DragComponent_SetStates(ref bool canDrag, ref DragAreaTypes dragFrom);
    float DragComponent_DragMinDistance { get; }
    float DragComponent_DragMaxDistance { get; }
    void DragComponent_DragOutEffects();
}

public enum DragAreaTypes
{
    None = 0,
    Bag = 1,
    MechaEditorArea = 2,
    MechaInside = 3,
    DiscardedArea = 4,
}