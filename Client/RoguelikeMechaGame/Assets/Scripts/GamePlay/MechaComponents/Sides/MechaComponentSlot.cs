using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class MechaComponentSlot : MonoBehaviour
{
    [SerializeField] private Material[] SlotLightMaterials;

    [SerializeField] private SlotType slotType;

    public SlotType SlotType
    {
        get { return slotType; }
        set
        {
            if (slotType != value)
            {
                OnChangeSlotType(value);
            }

            slotType = value;
        }
    }

    [SerializeField] private MeshRenderer SlotLightRenderer;

    public void OnChangeSlotType(SlotType slotType)
    {
        SlotLightRenderer.material = SlotLightMaterials[(int) slotType];
    }

    void Update()
    {
        if (!Application.isPlaying)
        {
            OnChangeSlotType(slotType);
        }
    }
}

public enum SlotType
{
    Yellow = 0,
    Red = 1,
    Blue = 2,
    Green = 3,
}