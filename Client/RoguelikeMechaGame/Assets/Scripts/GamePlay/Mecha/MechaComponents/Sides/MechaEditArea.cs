using UnityEngine;
using System.Collections;

public class MechaEditArea : MonoBehaviour
{
    [SerializeField] private MeshRenderer MeshRenderer;

    void Start()
    {
        MeshRenderer.enabled = false;
    }

    public void Show()
    {
        MeshRenderer.enabled = true;
    }

    public void Hide()
    {
        MeshRenderer.enabled = false;
    }
}