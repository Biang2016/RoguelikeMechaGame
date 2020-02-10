using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : MonoBehaviour
{
    public BladeInfo BladeInfo;

    public void Initialize(BladeInfo bladeInfo)
    {
        BladeInfo = bladeInfo;
    }
}