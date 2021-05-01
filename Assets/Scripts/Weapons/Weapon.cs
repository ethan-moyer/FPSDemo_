using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    [SerializeField] protected WeaponData data;
    [SerializeField] protected int currentAmmo;

    protected Animator animator;
    protected PlayerCombatController controller;
    protected PlayerInputManager controls;
    protected Transform cam;
    protected float timeTillNextFire = 0f;

    public enum States { Busy, Idle, Firing };
    public States currentState;

    public int ID { get { return data.weaponID; } }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public virtual void SetUp(PlayerCombatController controller, Transform cam, PlayerInputManager controls)
    {
        this.controller = controller;
        this.cam = cam;
        this.controls = controls;
        timeTillNextFire = 0f;
        if (currentAmmo == -1)
            currentAmmo = data.maxAmmo;
    }

    //When you switch to this weapon.
    public IEnumerator Equip() 
    {
        gameObject.SetActive(true);
        timeTillNextFire = 0f;
        currentState = States.Busy;
        yield return new WaitForSeconds(0.25f);
        currentState = States.Idle;
    }

    //When you switch away from this weapon.
    public IEnumerator Unequip()
    {
        currentState = States.Busy;
        animator.SetTrigger("Unequip");
        yield return new WaitForSeconds(0.25f);
        gameObject.SetActive(false);
    }

    //When the weapon is fired.
    public virtual void Fire()
    {
        if (timeTillNextFire == 0f && currentState == States.Idle)
        {
            currentState = States.Firing;
            animator.SetTrigger("Fire");
            timeTillNextFire = 1 / data.rateOfFire;

            RaycastHit hit;
            controller.gameObject.layer = 2;
            if (Physics.Raycast(cam.position, cam.forward, out hit))
            {
                Debug.Log(hit.transform.name);
                if (hit.rigidbody != null)
                    hit.rigidbody.AddForce(cam.forward * 200f);
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