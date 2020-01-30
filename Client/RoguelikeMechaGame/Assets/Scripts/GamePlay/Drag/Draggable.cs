using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可拖动组件，应用于机甲部件
/// </summary>
public class Draggable : MonoBehaviour
{
    IDraggable caller;

    void Awake()
    {
        caller = GetComponent<IDraggable>();
    }

    [SerializeField] private bool canDrag;
    private DragAreaTypes dragFrom;
    private float dragMinDistance;
    private float dragMaxDistance;

    private bool isBegin = true;
    private Vector3 dragBeginPosition;
    private Vector3 mOffset;

    void Update()
    {
        if (!canDrag) return;
        if (IsOnDrag)
        {
            Vector3 cameraPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (isBegin)
            {
                mOffset = gameObject.transform.position - GetMouseAsWorldPoint();
                dragBeginPosition = cameraPosition;
                isBegin = false;
            }

            switch (dragFrom)
            {
                case DragAreaTypes.Bag:
                {
                    float draggedDistance = (transform.position - dragBeginPosition).magnitude;
                    if (draggedDistance < dragMinDistance) //不动
                    {
                    }
                    else if (draggedDistance < dragMaxDistance) //拖拽物体本身 
                    {
                        Vector3 newPos = GetMouseAsWorldPoint() + mOffset + new Vector3(0.5f, 0, 0.5f) * GameManager.GridSize;
                        transform.position = new Vector3(newPos.x, transform.position.y, newPos.z);
                    }
                    else //消失 （显示特效或其他逻辑）
                    {
                    }

                    caller.DragComponent_OnMousePressed(CheckMoveToArea()); //将鼠标悬停的区域告知拖动对象主体
                    break;
                }
            }
        }
    }

    [SerializeField] private bool _isOnDrag = false;

    public bool IsOnDrag
    {
        get { return _isOnDrag; }

        set
        {
            if (value) //鼠标按下
            {
                caller.DragComponent_SetStates(ref canDrag, ref dragFrom);
                if (canDrag)
                {
                    caller.DragComponent_OnMouseDown();
                    dragMinDistance = caller.DragComponent_DragMinDistance();
                    dragMaxDistance = caller.DragComponent_DragMaxDistance();
                    _isOnDrag = value;
                    caller.DragComponent_DragOutEffects();
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
                    //将鼠标放开的区域告知拖动对象主体，并提供拖动起始姿态信息以供还原
                    caller.DragComponent_OnMouseUp(CheckMoveToArea());
                    isBegin = true;
                    _isOnDrag = value;
                    DragManager.Instance.CurrentDrag = null;
                }
                else
                {
                    _isOnDrag = false;
                    DragManager.Instance.CurrentDrag = null;
                }
            }
        }
    }

    private Vector3 GetMouseAsWorldPoint()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    public DragAreaTypes CheckMoveToArea()
    {
        if (BagManager.Instance.IsMouseInsideBag) return DragAreaTypes.Bag;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit raycast, 100f, GameManager.Instance.LayerMask_DragAreas);
        if (raycast.collider != null)
        {
            DragArea da = raycast.collider.gameObject.GetComponent<DragArea>();
            if (da)
            {
                return da.M_DragAreaTypes;
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
    /// <param name="dragLastPosition"></param>
    void DragComponent_OnMousePressed(DragAreaTypes dragAreaTypes);

    /// <summary>
    /// 传达鼠标左键松开时的鼠标位置信息
    /// </summary>
    /// <param name="dragAreaTypes">移动到了哪个区域</param>
    /// <param name="dragLastPosition">移动的最后位置</param>
    /// <param name="dragBeginPosition">移动的初始位置</param>
    /// <param name="dragBeginQuaternion">被拖动对象的初始旋转</param>
    void DragComponent_OnMouseUp(DragAreaTypes dragAreaTypes);

    void DragComponent_SetStates(ref bool canDrag, ref DragAreaTypes dragFrom);
    float DragComponent_DragMinDistance();
    float DragComponent_DragMaxDistance();
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