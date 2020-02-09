using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class Mecha
{
    private void Initialize_Fighting(MechaInfo mechaInfo)
    {
    }

    public float Speed = 3f;

    void Update_Fighting()
    {
        if (GameManager.Instance.GetState() == GameState.Fighting)
        {
            float movement = 0.7f * Time.deltaTime * Speed;
            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(-movement, 0, -movement, Space.World);
            }

            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(movement, 0, movement, Space.World);
            }

            if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(-movement, 0, movement, Space.World);
            }

            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(movement, 0, -movement, Space.World);
            }
        }
    }

    void LateUpdate_Fighting()
    {
        RotateToMouseDirection();
    }

    private Quaternion lastRotationByMouse;

    private void RotateToMouseDirection()
    {
        Ray ray = GameManager.Instance.MainCamera.ScreenPointToRay(Input.mousePosition);
        Vector3 intersect = ClientUtils.GetIntersectWithLineAndPlane(ray.origin, ray.direction, Vector3.up, transform.position);
        Quaternion rotation = Quaternion.LookRotation(intersect - transform.position);
        if (Mathf.Abs((rotation.eulerAngles - lastRotationByMouse.eulerAngles).magnitude) > 0.5f)
        {
            lastRotationByMouse = rotation;
            transform.localRotation = Quaternion.Lerp(transform.rotation, rotation, 1);
        }
    }

    public void ExertComponentBuffs()
    {
        foreach (MechaComponentBase mcb in mechaComponents)
        {
            mcb.ExertEffectOnOtherComponents();
        }
    }

    public void RemoveAllComponentBuffs()
    {
        foreach (MechaComponentBase mcb in mechaComponents)
        {
            mcb.UnlinkAllBuffs();
        }
    }
}