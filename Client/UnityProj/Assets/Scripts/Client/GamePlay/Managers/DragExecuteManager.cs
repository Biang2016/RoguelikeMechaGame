using BiangStudio.DragHover;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.GamePlay.UI;
using BiangStudio.GridBackpack;
using BiangStudio.Singleton;
using Client;
using GameCore;
using UnityEngine;

public class DragExecuteManager : TSingletonBaseManager<DragExecuteManager>
{
    public void Init()
    {
        DragProcessor<BackpackItem> dragProcessor_BackpackItem = new DragProcessor<BackpackItem>();
        dragProcessor_BackpackItem.Init(
            UIManager.Instance.UICamera,
            LayerManager.Instance.LayerMask_BackpackItemHitBox,
            () => ControlManager.Instance.Building_MousePosition,
            delegate(BackpackItem bi, Collider collider, IDragProcessor dragProcessor) { },
            delegate(BackpackItem bi, Collider collider, IDragProcessor dragProcessor) { }
        );

        DragProcessor<MechaComponentDropSprite> dragProcessor_MechaComponentDropSprite = new DragProcessor<MechaComponentDropSprite>();
        dragProcessor_MechaComponentDropSprite.Init(
            CameraManager.Instance.MainCamera,
            LayerManager.Instance.LayerMask_ItemDropped,
            () => ControlManager.Instance.Building_MousePosition,
            delegate(MechaComponentDropSprite mcds, Collider collider, IDragProcessor dragProcessor)
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
            delegate(MechaComponentDropSprite mcds, Collider collider, IDragProcessor dragProcessor) { }
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
            () => ControlManager.Instance.Building_MousePosition,
            delegate(MechaComponentBase mcb, Collider collider, IDragProcessor dragProcessor) { },
            delegate(MechaComponentBase mcb, Collider collider, IDragProcessor dragProcessor) { }
        );
    }
}