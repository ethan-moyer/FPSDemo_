using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PropWeapon : Prop
{
    [SerializeField] private int weaponID;
    [SerializeField] public int ammo;
    [SerializeField] private bool shouldRespawn = false;
    [SerializeField] private float respawnTime = 20f;
    [SerializeField] private float lifetime = 30f;

    public int WeaponID => weaponID;
    public int Ammo { get { return ammo; } set { value = ammo; } }

    protected override void Awake()
    {
        base.Awake();
        if (shouldRespawn == false)
            StartCoroutine(DestroyAfterSeconds(lifetime));
    }

    private IEnumerator DestroyAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        RemoveWeapon();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            if (other.gameObject.GetComponent<PlayerController>().OnHitWeaponProp(weaponID, ammo))
                RemoveWeapon();
        }
    }
    
    public void RemoveWeapon()
    {
        if (shouldRespawn)
        {
            WeaponRespawner.SharedInstance.DisableWeapon(this, respawnTime);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
