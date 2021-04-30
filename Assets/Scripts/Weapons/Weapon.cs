using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] protected WeaponData data;
    [Header("Properties")]
    [SerializeField] protected int currentAmmo;

    protected Animator animator;
    protected PlayerCombatController controller;
    protected PlayerInputManager controls;
    protected Transform cam;

    public enum States { Busy, Idle };
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
        if (currentAmmo == -1)
            currentAmmo = data.maxAmmo;
    }

    //When you switch to this weapon.
    public IEnumerator Equip() 
    {
        currentState = States.Busy;
        gameObject.SetActive(true);
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
    public virtual IEnumerator Fire()
    {
        currentState = States.Busy;
        Debug.Log("Fire.");
        animator.SetTrigger("Fire");
        yield return new WaitForSeconds(data.rateOfFire);
        currentState = States.Idle;
    }
}