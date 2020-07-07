using BiangStudio.DragHover;
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

        public bool GetMousePosOnThisArea(Vector2 mousePos, out Vector3 pos)
        {
            pos = Vector3.zero;
            Ray ray = DragManager.Instance.GetDragProcessor<BackpackItem>().Camera.ScreenPointToRay(mousePos);
            Physics.Raycast(ray, out RaycastHit hit, 1000f, 1 << BoxCollider.gameObject.layer);
            if (hit.collider)
            {
                if (hit.collider == BoxCollider)
                {
                    pos = hit.point;
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