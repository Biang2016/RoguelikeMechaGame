using UnityEngine;
using System.Collections;

public class MechaComponentEditOperator : MonoBehaviour
{
    public bool IsSelected = false;

    void Update()
    {
        if (IsSelected)
        {
            if (Input.GetKeyUp(KeyCode.R))
            {
                Rotate();
            }
        }
    }

    public void Rotate()
    {
        Debug.Log("Rotate");
    }
}