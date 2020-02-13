using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControllerManager : MonoSingleton<ControllerManager>
{
    SortedDictionary<PlayerNumber, Controller> PlayerControllerDict = new SortedDictionary<PlayerNumber, Controller>();

    public void Initialize()
    {
        
    }
    
}