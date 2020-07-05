using UnityEngine;
using System.Collections;
using BiangStudio.GridBackpack;
using BiangStudio.ShapedInventory;
using UnityEngine.UI;

[ExecuteInEditMode]
public class BackpackItemDesignerHelper : MonoBehaviour
{
    private RectTransform rectTransform;
    public InventoryItem Data;

    [SerializeField] private Image Image;
    [SerializeField] private BackpackItemGridHitBoxRoot BackpackItemGridHitBoxRoot;

    void Awake()
    {
        rectTransform = transform as RectTransform;
    }

    void Update()
    {
        //BackpackItemGridHitBoxRoot.
    }
}