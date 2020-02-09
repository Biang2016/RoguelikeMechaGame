using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MechaEditArea : DragArea
{
    [SerializeField] private MeshRenderer MeshRenderer_Circle;
    [SerializeField] private MeshRenderer MeshRenderer_Grid;
    [SerializeField] private BoxCollider BoxCollider;

    void Start()
    {
        MeshRenderer_Circle.enabled = false;
        MeshRenderer_Grid.enabled = false;
        BoxCollider.enabled = false;
    }

    private bool onMouseDrag_Right = false;
    private Vector3 mouseDownPos_Right = Vector3.zero;
    private bool onMouseDrag_Left = false;
    private Vector3 mouseDownPos_Left = Vector3.zero;

    void Update()
    {
        if (GameManager.Instance.GetState() == GameState.Building)
        {
            if (DragManager.Instance.CurrentDrag == null)
            {
                // Mouse Right button drag for rotate view
                if (Input.GetMouseButtonDown(1))
                {
                    onMouseDrag_Right = true;
                    if (GetMousePosOnThisArea(out Vector3 pos))
                    {
                        mouseDownPos_Right = pos;
                    }
                }

                if (onMouseDrag_Right && Input.GetMouseButton(1))
                {
                    if (GetMousePosOnThisArea(out Vector3 pos))
                    {
                        Vector3 startVec = mouseDownPos_Right - transform.position;
                        Vector3 endVec = pos - transform.position;

                        float rotateAngle = Vector3.SignedAngle(startVec, endVec, transform.up);
                        if (Mathf.Abs(rotateAngle) > 3)
                        {
                            BattleManager.Instance.PlayerMecha.transform.Rotate(0, rotateAngle, 0);
                            mouseDownPos_Right = pos;
                        }
                    }
                    else
                    {
                        onMouseDrag_Right = false;
                        mouseDownPos_Right = Vector3.zero;
                    }
                }

                if (Input.GetMouseButtonUp(1))
                {
                    onMouseDrag_Right = false;
                    mouseDownPos_Right = Vector3.zero;
                }

                // Mouse Left button drag for move whole mecha
                if (Input.GetMouseButtonDown(0))
                {
                    onMouseDrag_Left = true;
                    if (GetMousePosOnThisArea(out Vector3 pos))
                    {
                        mouseDownPos_Left = pos;
                    }
                }

                if (onMouseDrag_Left && Input.GetMouseButton(0))
                {
                    if (GetMousePosOnThisArea(out Vector3 pos))
                    {
                        Vector3 delta = pos - mouseDownPos_Left;
                        Vector3 delta_local = BattleManager.Instance.PlayerMecha.transform.InverseTransformVector(delta);
                        GridPos delta_local_GP = GridPos.GetGridPosByPoint(delta_local + Vector3.one * GameManager.GridSize / 2f, 1);
                        if (delta_local_GP.x != 0 || delta_local_GP.z != 0)
                        {
                            BattleManager.Instance.PlayerMecha.MoveCenter(delta_local_GP);
                            mouseDownPos_Left = pos;
                        }
                    }
                    else
                    {
                        onMouseDrag_Left = false;
                        mouseDownPos_Left = Vector3.zero;
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    onMouseDrag_Left = false;
                    mouseDownPos_Left = Vector3.zero;
                }
            }
        }
    }

    private bool GetMousePosOnThisArea(out Vector3 pos)
    {
        pos = Vector3.zero;
        Ray ray = GameManager.Instance.MainCamera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit, 200f, GameManager.Instance.LayerMask_DragAreas);
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

    public void Show()
    {
        MeshRenderer_Circle.enabled = true;
        MeshRenderer_Grid.enabled = true;
        BoxCollider.enabled = true;
    }

    public void Hide()
    {
        MeshRenderer_Circle.enabled = false;
        MeshRenderer_Grid.enabled = false;
        BoxCollider.enabled = false;
    }
}