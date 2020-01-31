using UnityEngine;
using System.Collections;

public class DebugPanel : BaseUIForm
{
    void Awake()
    {
        UIType.InitUIType(
            isClearStack: false,
            isESCClose: false,
            isClickElsewhereClose: false,
            uiForms_Type: UIFormTypes.Fixed,
            uiForms_ShowMode: UIFormShowModes.Normal,
            uiForm_LucencyType: UIFormLucencyTypes.Penetrable);
    }

    public void AddSword()
    {
        BagManager.Instance.AddMechaComponentToBag(new MechaComponentInfo(MechaComponentType.Sword, new GridPos(-2, 3, GridPos.Orientation.Right)));
    }
    public void AddEngine()
    {
        BagManager.Instance.AddMechaComponentToBag(new MechaComponentInfo(MechaComponentType.Engine, new GridPos(-2, 3, GridPos.Orientation.Right)));
    }
}