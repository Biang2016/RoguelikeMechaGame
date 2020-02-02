using System;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    internal HitBoxRoot ParentHitBoxRoot;

    private void Awake()
    {
        ParentHitBoxRoot =GetComponentInParent<HitBoxRoot>();
    }

    private void OnCollisionEnter(Collision other)
    {
        //TODO hit by anything
    }
}