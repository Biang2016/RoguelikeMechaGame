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
        public UnityEvent OnLogicTick; // used by FlowCanvas

        private void Initialize_Fighting(MechaInfo mechaInfo)
        {
            TransformHelper.CurrentTransform.Position = new FixVector3(transform.position);
        }

        public float Speed = 3f;

        void LogicTick_Fighting()
        {
            Vector2 speed = Time.deltaTime * Speed * ControlManager.Instance.Battle_Move.normalized;
            speed = Quaternion.Euler(0f, 0f, 45f) * speed;
            MechaInfo.TransformInfo.Position += new FixVector3((Fix64) speed.x, Fix64.Zero, (Fix64) speed.y);
            RotateToMouseDirection();

            OnLogicTick?.Invoke();
            foreach (KeyValuePair<int, MechaComponentBase> kv in MechaComponentDict)
            {
                kv.Value.LogicTick();
            }
        }

        void Update_Fighting()
        {
            TransformHelper.Update(MechaInfo.TransformInfo, transform, Time.deltaTime);
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

            float nearFactor = 3f / (intersect - transform.position).magnitude;

            Quaternion rotation = Quaternion.LookRotation(intersect - transform.position);
            if (Mathf.Abs((rotation.eulerAngles - lastRotationByMouse.eulerAngles).magnitude) > 0.5f * nearFactor)
            {
                lastRotationByMouse = rotation;
                MechaInfo.TransformInfo.Rotation = new FixQuaternion(Quaternion.Lerp(transform.rotation, rotation, 1));
            }
        }
    }
}