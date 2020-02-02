using UnityEngine;

namespace GamePlay
{
    public class CameraFollow : MonoBehaviour
    {
        private Transform target;
        private Vector3 Offset;

        public void SetTarget(Transform target)
        {
            this.target = target;
            if (target)
            {
                Offset = this.target.position - transform.position;
            }
        }

        private void LateUpdate()
        {
            if (target)
            {
                transform.position = target.position - Offset;
            }
        }
    }
}