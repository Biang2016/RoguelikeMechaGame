using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MechaEditArea : DragArea
{
    [SerializeField] private MeshRenderer MeshRenderer_Circle;
    [SerializeField] private MeshRenderer MeshRenderer_Grid;

    void Start()
    {
        MeshRenderer_Circle.enabled = false;
        MeshRenderer_Grid.enabled = false;
    }

    private bool onMouseDrag = false;
    private Vector3 mouseDownPos = Vector3.zero;

    void Update()
    {
        if (DragManager.Instance.CurrentDrag == null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                onMouseDrag = true;
                if (GetMousePosOnThisArea(out Vector3 pos))
                {
                    mouseDownPos = pos;
                }
            }

            if (onMouseDrag && Input.GetMouseButton(0))
            {
                if (GetMousePosOnThisArea(out Vector3 pos))
                {
                    Vector3 startVec = mouseDownPos - transform.position;
                    Vector3 endVec = pos - transform.position;

                    float rotateAngle = Vector3.SignedAngle(startVec, endVec, transform.up);
                    GameManager.Instance.PlayerMecha.transform.Rotate(0, rotateAngle, 0);
                    mouseDownPos = pos;
                }
                else
                {
                    onMouseDrag = false;
                    mouseDownPos = Vector3.zero;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                onMouseDrag = false;
                mouseDownPos = Vector3.zero;
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
            if (hit.collider.gameObject == gameObject)
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
    }

    public void Hide()
    {
        MeshRenderer_Circle.enabled = false;
        MeshRenderer_Grid.enabled = false;
    }
}