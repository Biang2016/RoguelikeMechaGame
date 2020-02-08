using UnityEngine;
using System.Collections;

public class MechaComponent_PowerUp : MechaComponentBase
{
    void Start()
    {
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void ExertEffectOnOtherComponents()
    {
        base.ExertEffectOnOtherComponents();
        MechaComponentBase mcb = GameManager.Instance.PlayerMecha.GetMechaComponent<IBuff_PowerUp>();
        MechaComponentBuff buff = new MechaComponentBuff(typeof(IBuff_PowerUp), this, mcb, new Modifier(2, Sign.Multiply));
        buff.AddBuff();
    }
}