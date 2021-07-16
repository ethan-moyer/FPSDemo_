using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponAttack : MonoBehaviour
{
    public abstract void Attack(GameObject player, ModularWeapon weapon, Transform cam);
}
