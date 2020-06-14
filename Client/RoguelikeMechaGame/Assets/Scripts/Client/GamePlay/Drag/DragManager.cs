using System;
using UnityEngine;
using System.Collections;
using GameCore;

namespace Client
{
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
                        Physics.Raycast(ray, out RaycastHit hit, 1000f, GameManager.Instance.LayerMask_ComponentHitBox);
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
                        Physics.Raycast(ray, out RaycastHit hit, 1000f, GameManager.Instance.LayerMask_ComponentHitBox);
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

                    // Drag items dropped
                    if (!CurrentDrag)
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            Ray ray = GameManager.Instance.MainCamera.ScreenPointToRay(Input.mousePosition);
                            Physics.Raycast(ray, out RaycastHit hit, 1000f, GameManager.Instance.LayerMask_ItemDropped);
                            if (hit.collider)
                            {
                                MechaComponentDropSprite mcds = hit.collider.GetComponentInParent<MechaComponentDropSprite>();
                                if (mcds)
                                {
                                    MechaComponentBase mcb = MechaComponentBase.BaseInitialize(mcds.MechaComponentInfo.Clone(), BattleManager.Instance.PlayerMecha);
                                    GridPos gp = ClientUtils.GetGridPosByMousePos(BattleManager.Instance.PlayerMecha.transform, Vector3.up, GameManager.GridSize);
                                    mcb.SetGridPosition(gp);
                                    BattleManager.Instance.PlayerMecha.AddMechaComponent(mcb);
                                    CurrentDrag = mcb.GetComponent<Draggable>();
                                    CurrentDrag.IsOnDrag = true;
                                    mcds.PoolRecycle();
                                }
                            }
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
}