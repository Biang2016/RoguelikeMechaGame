using UnityEngine;

namespace GameCore
{
    public class MechaBaseAIAgent
    {
        public MechaBase MechaBase;

        public MechaBaseAIAgent(MechaBase mechaBase)
        {
            MechaBase = mechaBase;
        }

        public void Update()
        {
            if (EnableRotate) RotateTowardsTarget();
            if (EnableMove) MoveToDestination();
        }

        public bool EnableRotate = false;
        public float RotateSpeed;
        private Vector3 currentRotateTarget;

        public void SetRotateTarget(Vector3 target)
        {
            target.y = MechaBase.transform.position.y;
            currentRotateTarget = target;
        }

        public void RotateTowardsTarget()
        {
            Vector3 diff = currentRotateTarget - MechaBase.transform.position;
            Quaternion rotation = Quaternion.LookRotation(diff);
            MechaBase.transform.rotation = Quaternion.Lerp(MechaBase.transform.rotation, rotation, Time.deltaTime * diff.magnitude * RotateSpeed);
        }

        public bool EnableMove = false;
        public float MoveSpeed;
        private Vector3 currentDestination;

        public void SetDestination(Vector3 dest)
        {
            dest.y = MechaBase.transform.position.y;
            currentDestination = dest;
        }

        public void MoveToDestination()
        {
            Vector3 diff = currentDestination - MechaBase.transform.position;
            MechaBase.transform.position += MoveSpeed * Time.deltaTime * diff.normalized;
        }

    }
}