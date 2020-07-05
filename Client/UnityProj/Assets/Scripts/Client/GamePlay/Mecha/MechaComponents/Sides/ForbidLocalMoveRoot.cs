using UnityEngine;

namespace Client
{
    public class ForbidLocalMoveRoot : MonoBehaviour
    {
        void Update()
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }
    }
}