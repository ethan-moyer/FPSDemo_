using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponAttack : MonoBehaviour
{
    public abstract void Attack(PlayerController player, ModularWeapon weapon, Transform cam);
}
