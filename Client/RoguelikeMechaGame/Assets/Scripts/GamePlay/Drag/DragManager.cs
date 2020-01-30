using System;
using UnityEngine;
using System.Collections;

public class DragManager : MonoSingleton<DragManager>
{
    private DragManager()
    {
    }

    void Awake()
    {
    }

    internal MechaComponentBase CurrentDrag_MechaComponentBase;

    [SerializeField] private Draggable currentDrag;

    internal Draggable CurrentDrag
    {
        get { return currentDrag; }
        set
        {
            currentDrag = value;
            if (currentDrag == null)
            {
                CurrentDrag_MechaComponentBase = null;
            }
            else
            {
                CurrentDrag_MechaComponentBase = currentDrag.GetComponent<MechaComponentBase>();
            }
        }
    }

    [NonSerialized] public bool ForbidDrag = false;

    void Update()
    {
        if (!ForbidDrag)
        {
            CommonDrag();
        }
        else
        {
            CancelCurrentDrag();
        }
    }

    private void CommonDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!CurrentDrag)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(ray, out RaycastHit hit, 50f, GameManager.Instance.LayerMask_ComponentHitBox);
                if (hit.collider != null)
                {
                    HitBox hitBox = hit.collider.gameObject.GetComponent<HitBox>();
                    if (hitBox)
                    {
                        CurrentDrag_MechaComponentBase = hitBox.ParentHitBoxRoot.MechaComponentBase;
                        CurrentDrag = CurrentDrag_MechaComponentBase.gameObject.GetComponent<Draggable>();
                        CurrentDrag.IsOnDrag = true;
                    }
                    else
                    {
                        ResetCurrentDrag();
                    }
                }
                else
                {
                    ResetCurrentDrag();
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            ResetCurrentDrag();
        }
    }

    public void ResetCurrentDrag()
    {
        if (CurrentDrag)
        {
            CurrentDrag.IsOnDrag = false;
            CurrentDrag = null;
        }
    }

    public void CancelCurrentDrag()
    {
        if (CurrentDrag)
        {
            CurrentDrag.IsOnDrag = false;
            CurrentDrag = null;
        }
    }
}