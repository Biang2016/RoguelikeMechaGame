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

        public float Speed = 3f;

        void Update_Fighting()
        {
            if (GameStateManager.Instance.GetState() == GameState.Fighting)
            {
                Vector2 speed = Time.deltaTime * Speed * ControlManager.Instance.Battle_Move.normalized;
                speed = Quaternion.Euler(0f, 0f, 45f) * speed;
                transform.Translate(speed.x, 0, speed.y, Space.World);
            }
        }

        void FixedUpdate_Fighting()
        {
        }

        void LateUpdate_Fighting()
        {
            RotateToMouseDirection();
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
                transform.localRotation = Quaternion.Lerp(transform.rotation, rotation, 1);
            }
        }
    }
}