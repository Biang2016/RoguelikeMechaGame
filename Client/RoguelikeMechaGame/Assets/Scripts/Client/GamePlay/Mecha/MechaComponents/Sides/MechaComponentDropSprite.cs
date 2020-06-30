using GameCore;
using UnityEngine;

namespace Client
{
    public class MechaComponentDropSprite : PoolObject
    {
        [SerializeField] private SpriteRenderer SpriteRenderer;
        [SerializeField] private Collider Collider;

        public MechaComponentInfo MechaComponentInfo;

        public void Initialize(MechaComponentInfo mechaComponentInfo, Vector3 position)
        {
            MechaComponentInfo = mechaComponentInfo.Clone();
            Sprite sprite = BagManager.Instance.BagItemSpriteDict[mechaComponentInfo.BagItemSpriteKey];
            SpriteRenderer.sprite = sprite;
            transform.position = position;
        }

        void LateUpdate()
        {
            transform.LookAt(GameManager.Instance.MainCamera.transform);
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
            if (Input.GetButtonDown("Bag"))
            {
                //if (StayingMecha && StayingMecha.MechaInfo.MechaType == MechaType.Self)
                //{
                //    if (BagManager.Instance.AddMechaComponentToBag(MechaComponentInfo, out BagItem _))
                //    {
                //        PoolRecycle();
                //        BagManager.Instance.OpenBag();
                //    }
                //}
            }
        }
    }
}