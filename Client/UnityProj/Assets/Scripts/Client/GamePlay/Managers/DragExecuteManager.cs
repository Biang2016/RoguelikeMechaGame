using BiangStudio.DragHover;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.GamePlay.UI;
using BiangStudio.GridBackpack;
using BiangStudio.Singleton;
using Client;
using GameCore;
using UnityEngine;
using DragAreaDefines = GameCore.DragAreaDefines;

public class DragExecuteManager : TSingletonBaseManager<DragExecuteManager>
{
    public void Init()
    {
        DragProcessor<BackpackItem> dragProcessor_BackpackItem = new DragProcessor<BackpackItem>();
        dragProcessor_BackpackItem.Init(
            UIManager.Instance.UICamera,
            LayerManager.Instance.LayerMask_BackpackItemHitBox,
            (out Vector2 mouseScreenPos) =>
            {
                if (ControlManager.Instance.BuildingInputActionEnabled)
                {
                    mouseScreenPos = ControlManager.Instance.Building_MousePosition;
                    return true;
                }
                else
                {
                    mouseScreenPos = Vector2.zero;
                    return false;
                }
            },
            ScreenMousePositionToWorld_BackpackDragArea,
            delegate(BackpackItem bi, Collider collider, DragProcessor dragProcessor) { },
            delegate(BackpackItem bi, Collider collider, DragProcessor dragProcessor) { }
        );

        DragProcessor<MechaComponentDropSprite> dragProcessor_MechaComponentDropSprite = new DragProcessor<MechaComponentDropSprite>();
        dragProcessor_MechaComponentDropSprite.Init(
            CameraManager.Instance.MainCamera,
            LayerManager.Instance.LayerMask_ItemDropped,
            (out Vector2 mouseScreenPos) =>
            {
                if (ControlManager.Instance.BuildingInputActionEnabled)
                {
                    mouseScreenPos = ControlManager.Instance.Building_MousePosition;
                    return true;
                }
                else
                {
                    mouseScreenPos = Vector2.zero;
                    return false;
                }
            },
            ScreenMousePositionToWorld_MechaEditorInventory,
            delegate(MechaComponentDropSprite mcds, Collider collider, DragProcessor dragProcessor)
            {
                Ray ray = CameraManager.Instance.MainCamera.ScreenPointToRay(ControlManager.Instance.Building_MousePosition);
                MechaComponentInfo mci = mcds.MechaComponentInfo.Clone();
                ClientBattleManager.Instance.PlayerMecha.MechaInfo.AddMechaComponentInfo(mci, GridPosR.Zero);
                MechaComponentBase mcb = ClientBattleManager.Instance.PlayerMecha.MechaComponentDict[mci.GUID];
                GridPos gp = GridUtils.GetGridPosByMousePos(ClientBattleManager.Instance.PlayerMecha.transform, ray, Vector3.up, ConfigManager.GridSize);
                mci.InventoryItem.SetGridPosition(gp);
                DragManager.Instance.CurrentDrag = mcb.GetComponent<Draggable>();
                DragManager.Instance.CurrentDrag.SetOnDrag(true, collider, dragProcessor);
                mcds.PoolRecycle();
            },
            delegate(MechaComponentDropSprite mcds, Collider collider, DragProcessor dragProcessor) { }
        );

        DragManager.Instance.Init(
            () => ControlManager.Instance.Building_MouseLeft.Down,
            () => ControlManager.Instance.Building_MouseLeft.Up,
            Debug.LogError,
            LayerManager.Instance.LayerMask_DragAreas);
        DragProcessor<MechaComponentBase> dragProcessor_MechaComponentBase = new DragProcessor<MechaComponentBase>();
        dragProcessor_MechaComponentBase.Init(
            CameraManager.Instance.MainCamera,
            LayerManager.Instance.LayerMask_ComponentHitBox,
            (out Vector2 mouseScreenPos) =>
            {
                if (ControlManager.Instance.BuildingInputActionEnabled)
                {
                    mouseScreenPos = ControlManager.Instance.Building_MousePosition;
                    return true;
                }
                else
                {
                    mouseScreenPos = Vector2.zero;
                    return false;
                }
            }
            ,
            ScreenMousePositionToWorld_MechaEditorInventory,
            delegate(MechaComponentBase mcb, Collider collider, DragProcessor dragProcessor) { },
            delegate(MechaComponentBase mcb, Collider collider, DragProcessor dragProcessor) { }
        );
    }

    private bool ScreenMousePositionToWorld_MechaEditorInventory(out Vector3 pos_world, out Vector3 pos_local, out Vector3 pos_matrix, out GridPos gp_matrix)
    {
        if (ClientBattleManager.Instance.PlayerMecha.MechaEditArea.GetMousePosOnThisArea(out pos_world, out pos_local, out pos_matrix, out gp_matrix))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool ScreenMousePositionToWorld_BackpackDragArea(out Vector3 pos_world, out Vector3 pos_local, out Vector3 pos_matrix, out GridPos gp_matrix)
    {
        if (BackpackManager.Instance.GetBackPack(DragAreaDefines.BattleInventory.DragAreaName).BackpackPanel.BackpackDragArea.GetMousePosOnThisArea(out pos_world, out pos_local, out pos_matrix, out gp_matrix))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}