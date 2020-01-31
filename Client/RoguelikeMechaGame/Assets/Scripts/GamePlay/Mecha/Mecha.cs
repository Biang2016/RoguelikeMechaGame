using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mecha : PoolObject
{
    public MechaInfo MechaInfo;

    private List<MechaComponentBase> mechaComponents = new List<MechaComponentBase>();

    [SerializeField] private Transform MechaComponentContainer;

    public void Initialize(MechaInfo mechaInfo)
    {
        MechaInfo = mechaInfo;
        foreach (MechaComponentInfo mci in mechaInfo.MechaComponentInfos)
        {
            MechaComponentBase mcb = MechaComponentBase.BaseInitialize(mci, MechaComponentContainer, this);
            mechaComponents.Add(mcb);
        }
    }

    public float Speed = 3f;

    void Update()
    {
        if (MechaInfo.MechaType == MechaType.Self)
        {
            if (GameManager.Instance.GetState() == GameState.Fighting)
            {
                float movement = 0.7f * Time.deltaTime * Speed;
                if (Input.GetKey(KeyCode.A))
                {
                    transform.Translate(-movement, 0, -movement);
                }

                if (Input.GetKey(KeyCode.D))
                {
                    transform.Translate(movement, 0, movement);
                }

                if (Input.GetKey(KeyCode.W))
                {
                    transform.Translate(-movement, 0, movement);
                }

                if (Input.GetKey(KeyCode.S))
                {
                    transform.Translate(movement, 0, -movement);
                }

                if (Input.GetKeyUp(KeyCode.G))
                {
                    SlotLightsShown = !SlotLightsShown;
                    GridShown = !GridShown;
                }
            }
        }
    }

    private bool _slotLightsShown = false;

    public bool SlotLightsShown
    {
        get { return _slotLightsShown; }
        set
        {
            if (_slotLightsShown != value)
            {
                foreach (MechaComponentBase mcb in mechaComponents)
                {
                    mcb.MechaComponentGrids.SetSlotLightsShown(value);
                }
            }

            _slotLightsShown = value;
        }
    }

    private bool _gridShown = false;

    public bool GridShown
    {
        get { return _gridShown; }
        set
        {
            if (_gridShown != value)
            {
                foreach (MechaComponentBase mcb in mechaComponents)
                {
                    mcb.MechaComponentGrids.SetGridShown(value);
                }
            }

            _gridShown = value;
        }
    }
}