﻿using BiangStudio.CloneVariant;
using UnityEngine;

namespace Client
{
    public class UIBattleTipInfo : IClone<UIBattleTipInfo>
    {
        public uint HitMCB_GUID;
        public BattleTipType BattleTipType;
        public AttackerType AttackerType;
        public int DiffHP;
        public int ElementHP;
        public float Scale;
        public int ElementType;
        public string SpriteImagePath;
        public Color Color;
        public Vector3 StartPos;
        public Vector2 Offset;
        public Vector2 RandomRange;
        public float DisappearTime = 1.5f;

        public UIBattleTipInfo(uint hitMcbGuid, BattleTipType battleTipType, AttackerType attackerType, int diffHp, int elementHp, float scale, int elementType,
            string spriteImagePath, Color color, Vector3 startPos, Vector2 offset, Vector2 randomRange, float disappearTime)
        {
            HitMCB_GUID = hitMcbGuid;
            BattleTipType = battleTipType;
            AttackerType = attackerType;
            DiffHP = diffHp;
            ElementHP = elementHp;
            Scale = scale;
            ElementType = elementType;
            SpriteImagePath = spriteImagePath;
            Color = color;
            StartPos = startPos;
            Offset = offset;
            RandomRange = randomRange;
            DisappearTime = disappearTime;
        }

        public UIBattleTipInfo Clone()
        {
            return new UIBattleTipInfo(HitMCB_GUID, BattleTipType, AttackerType, DiffHP, ElementHP, Scale, ElementType, SpriteImagePath, Color, StartPos, Offset, RandomRange, DisappearTime);
        }
    }
}