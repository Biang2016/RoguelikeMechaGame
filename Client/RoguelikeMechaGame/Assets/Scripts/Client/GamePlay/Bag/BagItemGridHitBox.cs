using GameCore;
using UnityEngine;

namespace Client
{
    public class BagItemGridHitBox : PoolObject
    {
        [SerializeField] private BoxCollider BoxCollider;

        public void Initialize(IntRect space)
        {
            BoxCollider.size = new Vector3(space.width, space.height, 1);
            BoxCollider.center = new Vector3((space.x + 0.5f) * space.width, (space.z - 0.5f) * space.height, 0);
        }
    }
}