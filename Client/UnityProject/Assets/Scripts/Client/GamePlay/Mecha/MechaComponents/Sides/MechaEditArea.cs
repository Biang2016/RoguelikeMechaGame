using BiangStudio.DragHover;
using BiangStudio.GameDataFormat.Grid;
using GameCore;
using UnityEngine;
using DragAreaDefines = GameCore.DragAreaDefines;

namespace Client
{
    [RequireComponent(typeof(BoxCollider))]
    public class MechaEditArea : DragAreaIndicator
    {
        [SerializeField]
        private MeshRenderer MeshRenderer_Range;

        [SerializeField]
        private MeshRenderer MeshRenderer_Grid;

        [SerializeField]
        private BoxCollider BoxCollider;

        [SerializeField]
        private GameObject PivotIndicator;

        [SerializeField]
        private MechaEditorAreaGridRoot MechaEditorAreaGridRoot;

        [SerializeField]
        private Light MechaEditorLight;

        private DragProcessor DragProcessor;

        void Start()
        {
            SetShown(false);
        }

        public void Init(MechaInfo mechaInfo)
        {
            DragProcessor = DragManager.Instance.GetDragProcessor<MechaComponent>();
            Clear();
            SetShown(false);
            DragArea.DragAreaName = mechaInfo.MechaEditorInventory.DragArea.DragAreaName;
            MechaEditorAreaGridRoot.Init();
        }

        public void Clear()
        {
            MechaEditorAreaGridRoot.Clear();
        }

        private bool onMouseDrag_Right = false;
        private Vector3 mouseDownPos_Right = Vector3.zero;
        private bool onMouseDrag_Left = false;
        private Vector3 mouseDownPos_Left = Vector3.zero;

        public void Update()
        {
            if (GameStateManager.Instance.GetState() == GameState.Building)
            {
                if (DragManager.Instance.CurrentDrag == null && DragManager.Instance.Current_DragArea.Equals(DragAreaDefines.MechaEditorArea))
                {
                    // Mouse Right button drag for rotate view
                    if (ControlManager.Instance.Building_MouseRight.Down)
                    {
                        onMouseDrag_Right = true;
                        if (GetMousePosOnThisArea(out Vector3 pos_world, out Vector3 _, out Vector3 _, out GridPos _))
                        {
                            mouseDownPos_Right = pos_world;
                        }
                    }

                    if (onMouseDrag_Right && ControlManager.Instance.Building_MouseRight.Pressed)
                    {
                        if (GetMousePosOnThisArea(out Vector3 pos_world, out Vector3 _, out Vector3 _, out GridPos _))
                        {
                            Vector3 startVec = mouseDownPos_Right - transform.position;
                            Vector3 endVec = pos_world - transform.position;

                            float rotateAngle = Vector3.SignedAngle(startVec, endVec, transform.up);
                            if (Mathf.Abs(rotateAngle) > 3)
                            {
                                ClientBattleManager.Instance.PlayerMecha.transform.Rotate(0, rotateAngle, 0);
                                mouseDownPos_Right = pos_world;
                            }
                        }
                        else
                        {
                            onMouseDrag_Right = false;
                            mouseDownPos_Right = Vector3.zero;
                        }
                    }

                    if (ControlManager.Instance.Building_MouseRight.Up)
                    {
                        onMouseDrag_Right = false;
                        mouseDownPos_Right = Vector3.zero;
                    }

                    // Mouse Left button drag for move whole mecha
                    if (ControlManager.Instance.Building_MouseLeft.Down)
                    {
                        onMouseDrag_Left = true;
                        if (GetMousePosOnThisArea(out Vector3 pos, out Vector3 _, out Vector3 _, out GridPos _))
                        {
                            mouseDownPos_Left = pos;
                        }
                    }

                    if (onMouseDrag_Left && ControlManager.Instance.Building_MouseLeft.Pressed)
                    {
                        if (GetMousePosOnThisArea(out Vector3 pos, out Vector3 _, out Vector3 _, out GridPos _))
                        {
                            Vector3 delta = pos - mouseDownPos_Left;
                            Vector3 delta_local = ClientBattleManager.Instance.PlayerMecha.transform.InverseTransformVector(delta);
                            GridPos delta_local_GP = GridPos.GetGridPosByPointXZ(delta_local, 1);
                            if (delta_local_GP.x != 0 || delta_local_GP.z != 0)
                            {
                                ClientBattleManager.Instance.PlayerMecha.MechaInfo.MechaEditorInventory.MoveAllItemTogether(delta_local_GP);
                                mouseDownPos_Left = pos;
                            }
                        }
                        else
                        {
                            onMouseDrag_Left = false;
                            mouseDownPos_Left = Vector3.zero;
                        }
                    }

                    if (ControlManager.Instance.Building_MouseLeft.Up)
                    {
                        onMouseDrag_Left = false;
                        mouseDownPos_Left = Vector3.zero;
                    }
                }
            }
        }

        public bool GetMousePosOnThisArea(out Vector3 pos_world, out Vector3 pos_local, out Vector3 pos_matrix, out GridPos gp_matrix)
        {
            pos_world = Vector3.zero;
            pos_local = Vector3.zero;
            pos_matrix = Vector3.zero;
            gp_matrix = GridPos.Zero;
            Ray ray = DragProcessor.Camera.ScreenPointToRay(DragProcessor.CurrentMousePosition_Screen);
            Physics.Raycast(ray, out RaycastHit hit, 1000f, LayerManager.Instance.LayerMask_DragAreas);
            if (hit.collider)
            {
                if (hit.collider == BoxCollider)
                {
                    pos_world = hit.point;
                    //todo!!!!!
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        public void SetShown(bool shown)
        {
            MeshRenderer_Range.enabled = shown;
            MeshRenderer_Grid.enabled = shown;
            BoxCollider.enabled = shown;
            PivotIndicator.SetActive(shown);
            MechaEditorAreaGridRoot.gameObject.SetActive(shown);
            MechaEditorLight.enabled = shown;
        }
    }
}