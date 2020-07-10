using System;
using BiangStudio.GameDataFormat;
using UnityEngine;

namespace GameCore
{
    public class TransformHelper
    {
        public TransformInfo CurrentTransform;
        public TransformInfo TargetTransform;

        public void Update(TransformInfo targetTransform, Transform transform, float deltaTime)
        {
            TargetTransform = targetTransform;
            CurrentTransform = CurrentTransform.Lerp(TargetTransform, deltaTime);
            transform.position = CurrentTransform.Position.ToVector3();
            transform.rotation = CurrentTransform.Rotation.ToQuaternion();
        }
    }

    [Serializable]
    public struct TransformInfo
    {
        public FixVector3 Position;
        public FixQuaternion Rotation;

        public TransformInfo Lerp(TransformInfo target, float deltaTime)
        {
            FixVector3 position = FixVector3.Lerp(Position, target.Position, (Fix64) (deltaTime * 20));
            FixQuaternion rotation = FixQuaternion.Lerp(Rotation, target.Rotation, (Fix64) (deltaTime * 20));
            return new TransformInfo {Position = position, Rotation = rotation};
        }

        public void Translate(float x, float y, float z)
        {
            Position += new FixVector3((Fix64) x, (Fix64) y, (Fix64) z);
        }

        public void Translate(Vector3 diff)
        {
            Position += new FixVector3((Fix64) diff.x, (Fix64) diff.y, (Fix64) diff.z);
        }
    }
}