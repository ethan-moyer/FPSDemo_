using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAttack : WeaponAttack
{
    [SerializeField] private float HPDamage = 0f;
    [SerializeField] private float SPMultiplier = 0f;
    [SerializeField] private int numberOfRays = 1;
    [SerializeField] private AudioClip firingClip = null;
    [SerializeField] private SniperTrail trail = null;

    public override void Attack(PlayerController player, ModularWeapon weapon, Transform cam)
    {
        weapon.PlayingEffect.Invoke();
        weapon.PlayingAudioClip.Invoke(firingClip);
        weapon.TriggeringAnimation.Invoke("Fire");

        player.gameObject.layer = 2;
        RaycastHit hit;
        for (int i = 0; i < numberOfRays; i++)
        {
            Vector3 randDir = (cam.forward * weapon.maxDistance) + (cam.right * Random.Range(-1f, 1f) * weapon.coneRadius) + (cam.up * Random.Range(-1f, 1f) * weapon.coneRadius);
            if (Physics.Raycast(cam.position, randDir, out hit, weapon.maxDistance))
            {
                if (hit.transform.gameObject.layer == 9) //Hit a player
                {
                    PlayerController otherPlayer = hit.transform.GetComponent<PlayerController>();
                    if (otherPlayer != null)
                    {
                        otherPlayer.DamageHit(HPDamage, SPMultiplier, hit.point, player);
                    }
                }
                else if (hit.transform.gameObject.layer == 12) //Hit a prop
                {
                    ModularWeapon.PlaceEffect(1, hit.point, hit.normal);
                    Prop prop = hit.transform.GetComponent<Prop>();
                    if (prop != null)
                    {
                        prop.Hit(hit.point, cam.forward * 10f);
                    }
                }
                else if (hit.transform.gameObject.layer == 10) //Hit terrain
                {
                    ModularWeapon.PlaceEffect(0, hit.point, hit.normal);
                }

                //Bullet Trail
                if (trail != null)
                {
                    trail.Positions = new Vector3[] { cam.TransformPoint(weapon.ViewModel.offset), hit.point };
                    trail.gameObject.SetActive(true);
                }
            }
            else
            {
                if (trail != null)
                {
                    trail.Positions = new Vector3[] { cam.TransformPoint(weapon.ViewModel.offset), weapon.ViewModel.offset + randDir * weapon.maxDistance };
                    trail.gameObject.SetActive(true);
                }
            }
        }
        player.gameObject.layer = 9;
    }
}
