using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Gun : Weapon
{
    [Header("Gun Attributes")]
    [SerializeField] private int magAmmo;
    [SerializeField] private float reloadTime;
    [SerializeField] private GameObject muzzleFlash;

    private enum States { Idle, Busy, Firing };
    private States currentState = States.Busy;

    public override bool CanSwitch => currentState == States.Idle;

    protected override void Awake()
    {
        base.Awake();
    }

    public override IEnumerator Equip()
    {
        gameObject.SetActive(true);
        timeTillNextFire = 0f;
        currentState = States.Busy;
        yield return new WaitForSeconds(0.25f);
        currentState = States.Idle;
    }

    public override IEnumerator Unequip()
    {
        currentState = States.Busy;
        animator.SetTrigger("Unequip");
        yield return new WaitForSeconds(0.25f);
        gameObject.SetActive(false);
    }

    public override void Fire()
    {
        if (timeTillNextFire == 0f && currentState == States.Idle)
        {
            currentState = States.Firing;
            muzzleFlash.GetComponent<VisualEffect>().Play();
            animator.SetTrigger("Fire");
            timeTillNextFire = 1 / fireRate;

            RaycastHit hit;
            controller.gameObject.layer = 2;
            if (Physics.Raycast(cam.position, cam.forward, out hit))
            {
                if (hit.transform.gameObject.layer == 12 && hit.transform.GetComponent<Prop>() != null)
                {
                    //Hit a prop.
                    hit.transform.GetComponent<Prop>().Hit(hit.point, cam.forward * 10f);
                    PlaceParticle(1, hit.point, hit.normal);
                }
                else if (hit.transform.gameObject.layer == 10 || hit.transform.gameObject.layer == 11)
                {
                    //Hit terrain or a platform.
                    PlaceParticle(0, hit.point, hit.normal);
                }
            }
            controller.gameObject.layer = 9;
        }
    }

    /// <summary>
    /// This method is used for placing particle effects from the object pooler onto a hit point.
    /// </summary>
    /// <param name="index">The index of the particle in the object pooler.</param>
    /// <param name="position">The position to place the particle.</param>
    /// <param name="normal">The normal vector of the hit surface.</param>
    private void PlaceParticle(int index, Vector3 position, Vector3 normal)
    {
        GameObject hitEffect = ObjectPooler.SharedInstance.GetPooledObject(index);
        hitEffect.transform.position = position;
        hitEffect.transform.rotation = Quaternion.LookRotation(normal);
        hitEffect.SetActive(true);
    }

    private void Update()
    {
        if (timeTillNextFire > 0f)
        {
            timeTillNextFire -= Time.deltaTime;
        }
        else if (currentState == States.Firing)
        {
            timeTillNextFire = 0f;
            currentState = States.Idle;
        }
    }
}
