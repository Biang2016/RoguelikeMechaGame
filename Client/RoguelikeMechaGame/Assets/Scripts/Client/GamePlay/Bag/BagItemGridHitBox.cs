using GameCore;
using UnityEngine;

namespace Client
{
    public class BagItemGridHitBox : PoolObject
    {
        public BoxCollider BoxCollider;

        public GridPos LocalGridPos;

        public void Initialize(GridPos localGP, GridRect space)
        {
            LocalGridPos = localGP;
            BoxCollider.size = new Vector3(space.width, space.height, 1);
            BoxCollider.center = new Vector3((space.x + 0.5f) * space.width, (space.z - 0.5f) * space.height, 0);
        }
    }
}