using BiangStudio.GameDataFormat.Grid;
using BiangStudio.GamePlay;
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
            BoxCollider.size = new Vector3(space.size.x, space.size.z, 1);
            BoxCollider.center = new Vector3((space.position.x + 0.5f) * space.size.x, (space.position.z - 0.5f) * space.size.z, 0);
        }
    }
}