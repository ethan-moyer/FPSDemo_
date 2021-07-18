using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRespawner : MonoBehaviour
{
    public static WeaponRespawner SharedInstance;

    private void Awake()
    {
        if (SharedInstance == null)
        {
            SharedInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DisableWeapon(PropWeapon weapon, float respawnTime)
    {
        StartCoroutine(RespawnWeapon(weapon, respawnTime));
    }

    private IEnumerator RespawnWeapon(PropWeapon weapon, float respawnTime)
    {
        print("Disabled");
        weapon.gameObject.SetActive(false);
        yield return new WaitForSeconds(respawnTime);
        print("Renabled");
        weapon.gameObject.SetActive(true);
    }
}
