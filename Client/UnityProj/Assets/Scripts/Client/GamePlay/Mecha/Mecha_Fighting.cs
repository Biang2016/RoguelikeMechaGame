using System.Collections.Generic;
using BiangStudio.GameDataFormat;
using BiangStudio.GameDataFormat.Grid;
using GameCore;
using UnityEngine;
using UnityEngine.Events;

namespace Client
{
    public partial class Mecha
    {
        private void Initialize_Fighting(MechaInfo mechaInfo)
        {
        }

        public float Speed = 6f;

        void Update_Fighting()
        {
            if (MechaInfo.MechaType == MechaType.Player)
            {
                Vector2 speed = Time.deltaTime * Speed * ControlManager.Instance.Battle_Move.normalized;
                speed = Quaternion.Euler(0f, 0f, 45f) * speed;
                transform.position += new Vector3(speed.x, 0, speed.y);
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


        public bool IsFriend(Mecha mecha)
        {
            if (MechaInfo.MechaType == mecha.MechaInfo.MechaType) return true;
            if (mecha.MechaInfo.MechaType == MechaType.Friend && MechaInfo.MechaType == MechaType.Player) return true;
            if (MechaInfo.MechaType == MechaType.Friend && mecha.MechaInfo.MechaType == MechaType.Player) return true;
            return false;
        }

        public bool IsMainPlayerFriend()
        {
            return MechaInfo.MechaType == MechaType.Friend;
        }

        public bool IsOpponent(Mecha mecha)
        {
            if (mecha.MechaInfo.MechaType == MechaType.Player && MechaInfo.MechaType == MechaType.Enemy) return true;
            if (MechaInfo.MechaType == MechaType.Enemy && mecha.MechaInfo.MechaType == MechaType.Player) return true;
            if (mecha.MechaInfo.MechaType == MechaType.Friend && MechaInfo.MechaType == MechaType.Enemy) return true;
            if (MechaInfo.MechaType == MechaType.Enemy && mecha.MechaInfo.MechaType == MechaType.Friend) return true;
            return false;
        }
    }
}