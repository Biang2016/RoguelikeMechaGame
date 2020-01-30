using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class MechaComponentEditOperator : MonoBehaviour
{
    public bool IsSelected = false;

    void Update()
    {
        if (IsSelected)
        {
            Rotate();
        }
    }

    public void Rotate()
    {
        transform.Rotate(0, 90, 0);
    }
}