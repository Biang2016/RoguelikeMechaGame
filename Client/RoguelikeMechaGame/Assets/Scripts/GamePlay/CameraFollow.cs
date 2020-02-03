using DG.Tweening;
using UnityEngine;

namespace GamePlay
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Camera Camera;
        private Transform target;
        private Vector3 Offset;
        [SerializeField] private float MinFOW;
        [SerializeField] private float MaxFOW;

        void Awake()
        {
            FOW_Level = 2;
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
            if (target)
            {
                Offset = this.target.position - transform.position;
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
            if (target)
            {
                transform.position = target.position - Offset;
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
    }
}