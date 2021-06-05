using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropWeapon : Prop
{
    [SerializeField] private int weaponID;
    [SerializeField] public int ammo;

    public int WeaponID => weaponID;
    public int Ammo { get { return ammo; } set { value = ammo; } }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            if (other.gameObject.GetComponent<PlayerController>().OnHitWeaponProp(weaponID, ammo))
                Destroy(this.gameObject);
        }
    }
}
