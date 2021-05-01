using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/WeaponData", order = 1)]
public class WeaponData : ScriptableObject
{
    [Header("Base Attributes")]
    public string weaponName;
    public int weaponID;
    public Sprite crosshair;
    public float damage;
    public float rateOfFire;
    public int maxAmmo;
    [Header("Reloadable Attributes")]
    public int magAmmo;
    public float reloadTime;
}
