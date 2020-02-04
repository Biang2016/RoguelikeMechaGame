using DG.Tweening;
using UnityEngine;

namespace GamePlay
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Camera Camera;
        private Transform target;
        private Vector3 targetingPoint;
        private Vector3 Offset_Fighting;
        private Vector3 Offset_Building;
        [SerializeField] private float MinFOW;
        [SerializeField] private float MaxFOW;

        void Awake()
        {
            RefreshTargetingPosition();
            FOW_Level = 2;
        }

        public void SetTarget(Transform _target)
        {
            target = _target;
            transform.position = transform.position + target.position - targetingPoint;
            RefreshTargetingPosition();

            if (target)
            {
                Offset_Fighting = target.position - targetingPoint;
                Offset_Building = target.position + transform.TransformVector(Vector3.right * 3f) - targetingPoint;
            }
        }

        private int _fow_Level = 0;

        internal int FOW_Level
        {
            get { return _fow_Level; }
            set
            {
                if (_fow_Level != value)
                {
                    _fow_Level = Mathf.Clamp(value, 0, 3);
                    Camera.DOFieldOfView(FOWs[_fow_Level], 0.2f);
                }
            }
        }

        private float[] FOWs = new float[] {10, 15, 25, 35};

        private void LateUpdate()
        {
            RefreshTargetingPosition();
            if (target)
            {
                switch (GameManager.Instance.GetState())
                {
                    case GameState.Building:
                    {
                        Vector3 destination = transform.position + target.position - targetingPoint + Offset_Building;
                        Vector3 curVelocity = Vector3.zero;
                        transform.position = Vector3.SmoothDamp(transform.position, destination, ref curVelocity, 0.05f);
                        break;
                    }
                    case GameState.Fighting:
                    {
                        Vector3 destination = transform.position + target.position - targetingPoint + Offset_Fighting;
                        Vector3 curVelocity = Vector3.zero;
                        transform.position = Vector3.SmoothDamp(transform.position, destination, ref curVelocity, 0.05f);
                        break;
                    }
                }
            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                FOW_Level++;
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                FOW_Level--;
            }
        }

        private void RefreshTargetingPosition()
        {
            Ray ray = GameManager.Instance.MainCamera.ScreenPointToRay(new Vector3(GameManager.Instance.MainCamera.pixelWidth / 2f, GameManager.Instance.MainCamera.pixelHeight / 2f));
            targetingPoint = ClientUtils.GetIntersectWithLineAndPlane(ray.origin, ray.direction, Vector3.up, Vector3.zero);
        }
    }
}