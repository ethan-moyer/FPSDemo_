using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttack : WeaponAttack
{
    [SerializeField] private Projectile projectile = null;
    [SerializeField] private float coneRadius = 1f;
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private Vector3 spawnOffset = Vector3.zero;
    [SerializeField] private AudioClip firingClip = null;

    public override void Attack(GameObject player, ModularWeapon weapon, Transform cam)
    {
        //Play visuals
        weapon.PlayingEffect.Invoke();
        weapon.PlayingAudioClip.Invoke(firingClip);
        weapon.TriggeringAnimation.Invoke("Fire");

        //Spawning new projectile
        Projectile spawnedProjectile = Instantiate(projectile, cam.TransformPoint(spawnOffset) + (player.GetComponent<CharacterController>().velocity * Time.deltaTime), Quaternion.LookRotation(cam.forward)).GetComponent<Projectile>();
        spawnedProjectile.player = player;
        Physics.IgnoreCollision(spawnedProjectile.GetComponent<Collider>(), player.GetComponent<Collider>());
        spawnedProjectile.gameObject.SetActive(true);

        //Change direction if looking at an object
        RaycastHit hit;
        Vector3 direction = cam.forward;
        player.layer = 2;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxDistance))
        {
            direction = (hit.point - cam.TransformPoint(spawnOffset)).normalized;
        }
        player.layer = 9;

        //Randomize direction within reticle cone
        Vector3 randDir = (direction * maxDistance) + (cam.right * Random.Range(-1f, 1f) * coneRadius) + (cam.up * Random.Range(-1f, 1f) * coneRadius);
        spawnedProjectile.direction = randDir.normalized;
    }
}
