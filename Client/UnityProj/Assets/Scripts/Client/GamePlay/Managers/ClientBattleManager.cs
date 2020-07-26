using System.Collections.Generic;
using System.Linq;
using BiangStudio.GamePlay.UI;
using BiangStudio.Singleton;
using GameCore;
using UnityEngine;

namespace Client
{
    public class ClientBattleManager : TSingletonBaseManager<ClientBattleManager>
    {
        internal Mecha PlayerMecha;
        internal SortedDictionary<uint, Mecha> MechaDict = new SortedDictionary<uint, Mecha>();

        private HUDPanel HUDPanel;
        public Transform MechaContainerRoot;
        public Transform MechaComponentDropSpriteContainerRoot;

        public void Clear()
        {
            foreach (KeyValuePair<uint, Mecha> kv in MechaDict)
            {
                kv.Value.PoolRecycle();
            }

            MechaDict.Clear();
            PlayerMecha = null;
        }

        public override void Awake()
        {
            MechaContainerRoot = new GameObject("MechaContainerRoot").transform;
            MechaComponentDropSpriteContainerRoot = new GameObject("MechaComponentDropSpriteContainerRoot").transform;
        }

        public override void Start()
        {
            HUDPanel = UIManager.Instance.ShowUIForms<HUDPanel>();
        }

        public override void Update()
        {
            Ray ray = CameraManager.Instance.MainCamera.ScreenPointToRay(ControlManager.Instance.Battle_MousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, LayerManager.Instance.LayerMask_ComponentHitBox))
            {
                if (hit.collider)
                {
                    MechaComponentHitBox hitBox = hit.collider.GetComponent<MechaComponentHitBox>();
                    if (hitBox.Mecha != null)
                    {
                        if (hitBox.Mecha.MechaInfo.MechaType == MechaType.Enemy)
                        {
                            HUDPanel.LoadEnemyMech(hitBox.Mecha);
                        }
                    }
                }
                else
                {
                    HUDPanel.LoadEnemyMech(null);
                }
            }
            else
            {
                HUDPanel.LoadEnemyMech(null);
            }
        }

        public void StartBattle(BattleInfo battleInfo)
        {
            Clear();
            InitBattleManagerProxy();

            battleInfo.OnAddMechaInfoSuc = AddMecha;

            BattleManager.Instance.StartBattle(battleInfo);

            CameraManager.Instance.MainCameraFollow.SetTarget(PlayerMecha.transform);
            GameStateManager.Instance.SetState(GameState.Fighting);

            HUDPanel.Initialize();
            PlayerMecha.MechaInfo.RefreshHUDPanelCoreLifeSliderCount();
        }

        private void AddMecha(MechaInfo mechaInfo)
        {
            Mecha mecha = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.Mecha].AllocateGameObject<Mecha>(MechaContainerRoot);
            mecha.Initialize(mechaInfo);
            mecha.OnRemoveMechaSuc = RemoveMecha;
            MechaDict.Add(mecha.MechaInfo.GUID, mecha);
            if (mechaInfo.MechaType == MechaType.Player)
            {
                PlayerMecha = mecha;
            }
        }

        private void RemoveMecha(Mecha mecha)
        {
            MechaDict.Remove(mecha.MechaInfo.GUID);
            mecha.PoolRecycle();
        }

        public Mecha FindMecha(uint guid)
        {
            MechaDict.TryGetValue(guid, out Mecha mecha);
            return mecha;
        }

        public MechaComponentBase FindMechaComponentBase(uint guid)
        {
            foreach (KeyValuePair<uint, Mecha> kv in MechaDict)
            {
                if (kv.Value.MechaComponentDict.TryGetValue(guid, out MechaComponentBase mcb))
                {
                    return mcb;
                }
            }

            return null;
        }

        public void SetAllMechaShown(bool shown)
        {
            foreach (KeyValuePair<uint, Mecha> kv in MechaDict)
            {
                kv.Value.SetShown(shown);
            }
        }

        public void SetAllEnemyMechaShown(bool shown)
        {
            foreach (KeyValuePair<uint, Mecha> kv in MechaDict)
            {
                if (!kv.Value.IsPlayer)
                {
                    kv.Value.SetShown(shown);
                }
            }
        }

        #region Proxy

        private void InitBattleManagerProxy()
        {
            BattleManager.Instance.SearchRangeHandler = SearchRangeDelegate;
        }

        private List<MechaComponentInfo> SearchRangeDelegate(Vector3 center, float radius)
        {
            Dictionary<uint, MechaComponentInfo> res = new Dictionary<uint, MechaComponentInfo>();
            Collider[] colliders = Physics.OverlapSphere(center, radius, LayerManager.Instance.LayerMask_ComponentHitBox, QueryTriggerInteraction.Collide);
            foreach (Collider collider in colliders)
            {
                MechaComponentBase mcb = collider.GetComponentInParent<MechaComponentBase>();
                if (mcb.IsAlive())
                {
                    if (!res.ContainsKey(mcb.MechaComponentInfo.GUID))
                    {
                        res.Add(mcb.MechaComponentInfo.GUID, mcb.MechaComponentInfo);
                    }
                }
            }

            return res.Values.ToList();
        }

        #endregion
    }
}