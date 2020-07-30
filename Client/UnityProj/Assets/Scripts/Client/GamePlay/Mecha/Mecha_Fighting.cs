using System.Collections.Generic;
using BiangStudio.GameDataFormat.Grid;
using GameCore;
using UnityEngine;

namespace Client
{
    public partial class Mecha
    {
        private void Initialize_Fighting(MechaInfo mechaInfo)
        {
        }

        public bool AbilityForbidMovement = false;
        public float Speed = 6f;

        void Update_Fighting()
        {
            AbilityForbidMovement = false;
            foreach (KeyValuePair<uint, MechaComponentBase> kv in MechaComponentDict)
            {
                if (!kv.Value.IsRecycled)
                {
                    kv.Value.Update_Fighting();
                }
            }

            if (MechaInfo.MechaType == MechaType.Player)
            {
                if (!AbilityForbidMovement)
                {
                    Vector2 speed = Time.deltaTime * Speed * ControlManager.Instance.Battle_Move.normalized;
                    speed = Quaternion.Euler(0f, 0f, 45f) * speed;
                    transform.position += new Vector3(speed.x, 0, speed.y);
                }

                RotateToMouseDirection();
            }
        }

        void FixedUpdate_Fighting()
        {
        }

        void LateUpdate_Fighting()
        {
        }

        private Quaternion lastRotationByMouse;

        private void RotateToMouseDirection()
        {
            Ray ray = CameraManager.Instance.MainCamera.ScreenPointToRay(ControlManager.Instance.Battle_MousePosition);
            Vector3 intersect = GridUtils.GetIntersectWithLineAndPlane(ray.origin, ray.direction, Vector3.up, transform.position);

            Vector3 diff = intersect - transform.position;
            float nearFactor = 3f / (diff).magnitude;

            Quaternion rotation = Quaternion.LookRotation(diff);
            if (Mathf.Abs((rotation.eulerAngles - lastRotationByMouse.eulerAngles).magnitude) < 0.5f * nearFactor)
            {
                rotation = transform.rotation;
            }

            lastRotationByMouse = transform.rotation;
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * diff.magnitude * 3f);
        }
    }
}