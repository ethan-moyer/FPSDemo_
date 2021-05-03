using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon
{
    [Header("Gun Attributes")]
    [SerializeField] private int magAmmo;
    [SerializeField] private float reloadTime;

    private enum States { Idle, Busy, Firing };
    private States currentState = States.Busy;

    public override bool CanSwitch => currentState == States.Idle;

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
            animator.SetTrigger("Fire");
            timeTillNextFire = 1 / fireRate;

            RaycastHit hit;
            controller.gameObject.layer = 2;
            if (Physics.Raycast(cam.position, cam.forward, out hit))
            {
                if (hit.transform.gameObject.layer == 12 && hit.transform.GetComponent<Prop>() != null)
                    hit.transform.GetComponent<Prop>().Hit(hit.point, cam.forward);
            }
            controller.gameObject.layer = 9;
        }
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
