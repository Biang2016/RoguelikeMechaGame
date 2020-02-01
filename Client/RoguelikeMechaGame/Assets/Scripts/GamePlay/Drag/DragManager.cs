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
    internal BagItem CurrentDrag_BagItem;

    [SerializeField] private Draggable currentDrag;

    internal Draggable CurrentDrag
    {
        get { return currentDrag; }
        set
        {
            if (currentDrag != value)
            {
                currentDrag = value;
                CurrentDrag_MechaComponentBase = currentDrag ? currentDrag.GetComponent<MechaComponentBase>() : null;
                CurrentDrag_BagItem = currentDrag ? currentDrag.GetComponent<BagItem>() : null;
            }
        }
    }

    [NonSerialized] public bool ForbidDrag = false;

    void Update()
    {
        if (ForbidDrag)
        {
            CancelCurrentDrag();
        }
        else
        {
            CommonDrag();
        }
    }

    private void CommonDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!CurrentDrag)
            {
                {
                    // Drag items in bag
                    Ray ray = UIManager.Instance.UICamera.ScreenPointToRay(Input.mousePosition);
                    Physics.Raycast(ray, out RaycastHit hit, 50f, GameManager.Instance.LayerMask_ComponentHitBox);
                    Debug.DrawRay(ray.origin, ray.direction * 50f, Color.green);
                    if (hit.collider)
                    {
                        BagItem bagItem = hit.collider.gameObject.GetComponentInParent<BagItem>();
                        if (bagItem)
                        {
                            CurrentDrag = bagItem.gameObject.GetComponent<Draggable>();
                            CurrentDrag.IsOnDrag = true;
                        }
                        else
                        {
                            CancelCurrentDrag();
                        }
                    }
                    else
                    {
                        CancelCurrentDrag();
                    }
                }

                //Drag components in scene
                if (!CurrentDrag)
                {
                    Ray ray = GameManager.Instance.MainCamera.ScreenPointToRay(Input.mousePosition);
                    Physics.Raycast(ray, out RaycastHit hit, 50f, GameManager.Instance.LayerMask_ComponentHitBox);
                    if (hit.collider)
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
                            CancelCurrentDrag();
                        }
                    }
                    else
                    {
                        CancelCurrentDrag();
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            CancelCurrentDrag();
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

    internal bool IsMouseInsideBag = false;
}