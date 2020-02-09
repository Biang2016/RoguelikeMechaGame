using UnityEngine;

public class MechaComponentDropSprite : PoolObject
{
    [SerializeField] private SpriteRenderer SpriteRenderer;

    public void Initialize(MechaComponentType mechaComponentType,Vector3 position)
    {
        Sprite sprite = BagManager.Instance.MechaComponentSpriteDict[mechaComponentType];
        SpriteRenderer.sprite = sprite;
        transform.position = position;
    }

    void LateUpdate()
    {
        transform.LookAt(GameManager.Instance.MainCamera.transform);
    }
}