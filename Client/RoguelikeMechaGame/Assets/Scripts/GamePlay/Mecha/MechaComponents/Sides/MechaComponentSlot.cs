using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class MechaComponentSlot : MonoBehaviour
{
    [SerializeField] private MeshRenderer SlotLightRenderer;
    [SerializeField] private Material[] SlotLightMaterials;

    [SerializeField] private SlotType _slotType;
    internal GridPos.Orientation Orientation;

    public SlotType SlotType
    {
        get { return _slotType; }
        set
        {
            if (_slotType != value)
            {
                OnChangeSlotType(value);
            }

            _slotType = value;
        }
    }

    public void OnChangeSlotType(SlotType slotType)
    {
        SlotLightRenderer.material = SlotLightMaterials[(int) slotType];
    }

    public void Initialize()
    {
        Orientation = GridPos.GetGridPosByLocalTrans(transform, GameManager.GridSize).orientation;
    }

    void Update()
    {
        if (!Application.isPlaying)
        {
            OnChangeSlotType(_slotType);
        }
    }

    public void SetShown(bool shown)
    {
        SlotLightRenderer.enabled = shown;
    }
}

public enum SlotType
{
    Yellow = 0,
    Red = 1,
    Blue = 2,
    Green = 3,
}