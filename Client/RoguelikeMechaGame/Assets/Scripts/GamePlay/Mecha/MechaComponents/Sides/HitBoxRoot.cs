using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HitBoxRoot : MonoBehaviour
{
    internal MechaComponentBase MechaComponentBase;
    internal List<HitBox> HitBoxes = new List<HitBox>();

    void Awake()
    {
        MechaComponentBase = GetComponentInParent<MechaComponentBase>();
        HitBoxes = GetComponentsInChildren<HitBox>().ToList();
    }


    public void SetInBattle(bool inBattle)
    {
        foreach (HitBox hitBox in HitBoxes)
        {
            hitBox.SetInBattle(inBattle);
        }
    }

}