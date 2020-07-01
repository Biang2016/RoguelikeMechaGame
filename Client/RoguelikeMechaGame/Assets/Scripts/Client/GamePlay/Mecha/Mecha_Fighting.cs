using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameCore;

namespace Client
{
    public partial class Mecha
    {
        private void Initialize_Fighting(MechaInfo mechaInfo)
        {
        }

        public float Speed = 3f;

        void FixedUpdate_Fighting()
        {
            if (GameManager.Instance.GetState() == GameState.Fighting)
            {
                float movement = 0.7f * Time.deltaTime * Speed;
                if (ControlManager.Instance.Battle_Move.x < 0)
                {
                    transform.Translate(-movement, 0, -movement, Space.World);
                }
                else if (ControlManager.Instance.Battle_Move.x > 0)
                {
                    transform.Translate(movement, 0, movement, Space.World);
                }

                if (ControlManager.Instance.Battle_Move.y < 0)
                {
                    transform.Translate(movement, 0, -movement, Space.World);
                }
                else if (ControlManager.Instance.Battle_Move.y > 0)
                {
                    transform.Translate(-movement, 0, movement, Space.World);
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
            Ray ray = GameManager.Instance.MainCamera.ScreenPointToRay(ControlManager.Instance.Common_MousePosition);
            Vector3 intersect = ClientUtils.GetIntersectWithLineAndPlane(ray.origin, ray.direction, Vector3.up, transform.position);

            float nearFactor = 3f / (intersect - transform.position).magnitude;

            Quaternion rotation = Quaternion.LookRotation(intersect - transform.position);
            if (Mathf.Abs((rotation.eulerAngles - lastRotationByMouse.eulerAngles).magnitude) > 0.5f * nearFactor)
            {
                lastRotationByMouse = rotation;
                transform.localRotation = Quaternion.Lerp(transform.rotation, rotation, 1);
            }
        }
    }
}