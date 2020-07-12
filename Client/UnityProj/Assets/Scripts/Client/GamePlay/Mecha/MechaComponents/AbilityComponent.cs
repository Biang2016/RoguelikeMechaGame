using Client;
using GameCore.AbilityDataDriven;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(MechaComponentBase))]
public class AbilityComponent : MonoBehaviour
{
    [LabelText("技能编辑区")]
    public Ability Ability;
}