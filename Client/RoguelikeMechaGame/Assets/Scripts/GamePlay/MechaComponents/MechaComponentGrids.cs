using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
public class MechaComponentGrids : MonoBehaviour
{
    private List<MechaComponentGrid> M_MechaComponentGrids = new List<MechaComponentGrid>();

    void Awake()
    {
        M_MechaComponentGrids = GetComponentsInChildren<MechaComponentGrid>().ToList();
    }
}