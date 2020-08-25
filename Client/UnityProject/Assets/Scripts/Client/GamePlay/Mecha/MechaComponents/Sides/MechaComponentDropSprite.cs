using BiangStudio.GridBackpack;
using BiangStudio.ObjectPool;
using GameCore;
using UnityEngine;

namespace Client
{
    public class MechaComponentDropSprite : PoolObject
    {
        [SerializeField]
        private SpriteRenderer SpriteRenderer;

        [SerializeField]
        private Collider Collider;

        public MechaComponentInfo MechaComponentInfo;

        public void Initialize(MechaComponentInfo mechaComponentInfo, Vector3 position)
        {
            MechaComponentInfo = mechaComponentInfo.Clone();
            Sprite sprite = BackpackManager.Instance.GetBackpackItemSprite(mechaComponentInfo.ItemSpriteKey);
            SpriteRenderer.sprite = sprite;
            transform.position = position;
        }

        void LateUpdate()
        {
            transform.LookAt(CameraManager.Instance.MainCamera.transform);
        }

        private Mecha StayingMecha = null;

        private void OnTriggerEnter(Collider c)
        {
            StayingMecha = c.GetComponentInParent<Mecha>();
        }

        private void OnTriggerExit(Collider c)
        {
            StayingMecha = null;
        }

        private void OnTriggerStay(Collider c)
        {
            if (Input.GetButtonDown("Backpack"))
            {
                //if (StayingMecha && StayingMecha.MechaInfo.MechaCamp == MechaCamp.Self)
                //{
                //    if (BackpackManager.Instance.AddMechaComponentToBackpack(MechaComponentInfo, out BackpackItem _))
                //    {
                //        PoolRecycle();
                //        BackpackManager.Instance.OpenBackpack();
                //    }
                //}
            }
        }
    }
}