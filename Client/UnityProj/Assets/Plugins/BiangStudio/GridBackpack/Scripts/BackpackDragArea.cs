using BiangStudio.DragHover;
using BiangStudio.GameDataFormat.Grid;
using UnityEngine;

namespace BiangStudio.GridBackpack
{
    public class BackpackDragArea : DragAreaIndicator
    {
        private Backpack Backpack;
        private DragProcessor DragProcessor;

        [SerializeField] private BoxCollider BoxCollider;

        public void Init(Backpack backpack)
        {
            Backpack = backpack;
            DragArea = new DragArea(backpack.DragArea.DragAreaName);
            DragProcessor = DragManager.Instance.GetDragProcessor<BackpackItem>();
        }

        public bool GetMousePosOnThisArea(out Vector3 pos_world, out GridPos gp_matrix)
        {
            pos_world = Vector3.zero;
            gp_matrix = GridPos.Zero;
            Ray ray = DragProcessor.Camera.ScreenPointToRay(DragProcessor.CurrentMousePosition_Screen);
            Physics.Raycast(ray, out RaycastHit hit, 1000f, 1 << BoxCollider.gameObject.layer);
            if (hit.collider)
            {
                if (hit.collider == BoxCollider)
                {
                    pos_world = hit.point;
                    Vector3 localPos = Backpack.BackpackPanel.ItemContainer.transform.InverseTransformPoint(pos_world);
                    Vector2 containerSize = ((RectTransform) Backpack.BackpackPanel.ItemContainer).rect.size;
                    Vector3 local_matrix = new Vector3(localPos.x + containerSize.x / 2f - Backpack.GridSize/2f, containerSize.y / 2f - localPos.y - Backpack.GridSize / 2f);
                    gp_matrix = GridPos.GetGridPosByPointXY(local_matrix, Backpack.GridSize);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }
    }
}