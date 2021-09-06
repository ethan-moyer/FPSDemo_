using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttack : WeaponAttack
{
    [SerializeField] private Projectile projectile = null;
    [SerializeField] private Vector3 spawnOffset = Vector3.zero;
    [SerializeField] private AudioClip firingClip = null;
    [SerializeField] private bool usePooledProjectile = false;
    [SerializeField] private int poolIndex;

    public override void Attack(PlayerController player, ModularWeapon weapon, Transform cam)
    {
        //Play visuals
        weapon.PlayingEffect.Invoke();
        weapon.PlayingAudioClip.Invoke(firingClip);
        weapon.TriggeringAnimation.Invoke("Fire");

        //Spawning new projectile
        Projectile spawnedProjectile;
        if (usePooledProjectile)
        {
            spawnedProjectile = ObjectPooler.SharedInstance.GetPooledObject(poolIndex).GetComponent<Projectile>();
            spawnedProjectile.transform.position = cam.TransformPoint(spawnOffset) + (player.GetComponent<CharacterController>().velocity * Time.deltaTime);
            spawnedProjectile.transform.rotation = Quaternion.LookRotation(cam.forward);
        }
        else
        {
            spawnedProjectile = Instantiate(projectile, cam.TransformPoint(spawnOffset) + (player.GetComponent<CharacterController>().velocity * Time.deltaTime), Quaternion.LookRotation(cam.forward)).GetComponent<Projectile>();
        }

        spawnedProjectile.player = player;
        Physics.IgnoreCollision(spawnedProjectile.GetComponent<Collider>(), player.GetComponent<Collider>());
        spawnedProjectile.gameObject.SetActive(true);

        //Change direction if looking at an object
        RaycastHit hit;
        Vector3 direction = cam.forward;
        player.gameObject.layer = 2;
        if (Physics.Raycast(cam.position, cam.forward, out hit, weapon.maxDistance))
        {
            direction = (hit.point - cam.TransformPoint(spawnOffset)).normalized;
        }
        player.gameObject.layer = 9;

        //Randomize direction within reticle cone
        Vector3 randDir = (direction * weapon.maxDistance) + (cam.right * Random.Range(-1f, 1f) * weapon.coneRadius) + (cam.up * Random.Range(-1f, 1f) * weapon.coneRadius);
        spawnedProjectile.direction = randDir.normalized;
    }
}
