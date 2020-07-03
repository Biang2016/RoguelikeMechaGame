using System;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.GamePlay.UI;
using BiangStudio.Singleton;
using GameCore;
using UnityEngine;

namespace Client
{
    public class DragManager : TSingletonBaseManager<DragManager>
    {
        internal MechaComponentBase CurrentDrag_MechaComponentBase;
        internal BagItem CurrentDrag_BagItem;

        [SerializeField] private Draggable currentDrag;
        [NonSerialized] public bool ForbidDrag = false;
        internal bool IsMouseInsideBag = false;

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

        public override void Update()
        {
            Ray ray = CameraManager.Instance.MainCamera.ScreenPointToRay(ControlManager.Instance.Building_MousePosition);
            GridPos gp = GridUtils.GetGridPosByMousePos(BattleManager.Instance.PlayerMecha.transform, ray, Vector3.up, ConfigManager.GridSize);

            Debug.Log(gp);
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
            if (ControlManager.Instance.Building_MouseLeft.Down)
            {
                if (!CurrentDrag)
                {
                    {
                        // Drag items in bag
                        Ray ray = UIManager.Instance.UICamera.ScreenPointToRay(ControlManager.Instance.Building_MousePosition);
                        Physics.Raycast(ray, out RaycastHit hit, 1000f, LayerManager.Instance.LayerMask_ComponentHitBox);
                        if (hit.collider)
                        {
                            BagItem bagItem = hit.collider.gameObject.GetComponentInParent<BagItem>();
                            if (bagItem)
                            {
                                CurrentDrag = bagItem.gameObject.GetComponent<Draggable>();
                                CurrentDrag.SetOnDrag(true, hit.collider);
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
                        Ray ray = CameraManager.Instance.MainCamera.ScreenPointToRay(ControlManager.Instance.Building_MousePosition);
                        Physics.Raycast(ray, out RaycastHit hit, 1000f, LayerManager.Instance.LayerMask_ComponentHitBox);
                        if (hit.collider)
                        {
                            MechaComponentHitBox hitBox = hit.collider.gameObject.GetComponent<MechaComponentHitBox>();
                            if (hitBox)
                            {
                                CurrentDrag_MechaComponentBase = hitBox.ParentHitBoxRoot.MechaComponentBase;
                                CurrentDrag = CurrentDrag_MechaComponentBase.gameObject.GetComponent<Draggable>();
                                CurrentDrag.SetOnDrag(true, hit.collider);
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
                        Ray ray = CameraManager.Instance.MainCamera.ScreenPointToRay(ControlManager.Instance.Building_MousePosition);
                        Physics.Raycast(ray, out RaycastHit hit, 1000f, LayerManager.Instance.LayerMask_ItemDropped);
                        if (hit.collider)
                        {
                            MechaComponentDropSprite mcds = hit.collider.GetComponentInParent<MechaComponentDropSprite>();
                            if (mcds)
                            {
                                MechaComponentBase mcb = MechaComponentBase.BaseInitialize(mcds.MechaComponentInfo.Clone(), BattleManager.Instance.PlayerMecha);
                                GridPos gp = GridUtils.GetGridPosByMousePos(BattleManager.Instance.PlayerMecha.transform, ray, Vector3.up, ConfigManager.GridSize);
                                mcb.SetGridPosition(gp);
                                BattleManager.Instance.PlayerMecha.AddMechaComponent(mcb);
                                CurrentDrag = mcb.GetComponent<Draggable>();
                                CurrentDrag.SetOnDrag(true, hit.collider);
                                mcds.PoolRecycle();
                            }
                        }
                    }
                }
            }

            if (ControlManager.Instance.Building_MouseLeft.Up)
            {
                CancelCurrentDrag();
            }
        }

        public void CancelCurrentDrag()
        {
            if (CurrentDrag)
            {
                CurrentDrag.SetOnDrag(false, null);
                CurrentDrag = null;
            }
        }
    }
}