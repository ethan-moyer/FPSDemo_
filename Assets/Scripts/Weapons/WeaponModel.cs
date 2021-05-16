using System;
using UnityEngine;
using UnityEngine.VFX;

[Serializable]
public struct WeaponModel
{
    public Mesh mesh;
    public Material[] materials;
    public RuntimeAnimatorController animator;
    public Vector3 offset;
    public Vector3 scale;
}

[Serializable]
public struct WeaponEffect
{
    public VisualEffectAsset visualEffect;
    public Vector3 offset;
    public Vector3 scale;
}