using UnityEngine;
using System.Collections;
using Client;
using GameCore.AbilityDataDriven;
using Sirenix.OdinInspector;

[RequireComponent(typeof(MechaComponentBase))]
public class AbilityComponent : MonoBehaviour
{
    [LabelText("技能编辑区")]
    public Ability Ability;
}