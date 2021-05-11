using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon Identification")]
    [SerializeField] protected string weaponName = "Weapon";
    [SerializeField] protected int weaponID = 0;
    [Header("Aiming Attribtes")]
    [SerializeField] public Sprite reticle = null;
    [SerializeField] public float reticleSize = 0.1f;
    [SerializeField] public float maxRange = 80f;
    [SerializeField] protected float coneRadius = 5f;
    [Header("Firing Attributes")]
    [SerializeField] protected float damage = 0f;
    [SerializeField] protected float fireRate = 0f;
    [SerializeField] protected int currentAmmo = -1;
    [SerializeField] protected int maxAmmo = 0;

    protected Animator animator;
    protected PlayerCombatController controller;
    protected PlayerInputManager controls;
    protected Transform cam;
    protected float timeTillNextFire = 0f;
    protected enum States { Idle, Busy, Firing };
    protected States currentState;

    public int WeaponID => weaponID;
    public bool IsIdle => currentState == States.Idle;
    public float MaxAngle { get { return Vector3.Angle(cam.forward.normalized, cam.forward.normalized + cam.right.normalized); } }

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Handles tying this weapon to a player.
    /// </summary>
    /// <param name="controller">The PlayerCombatController to link it to.</param>
    /// <param name="cam">The camera of the player to link it to.</param>
    /// <param name="controls">The PlayerInputManager to link it to.</param>
    public virtual void SetUp(PlayerCombatController controller, Transform cam, PlayerInputManager controls)
    {
        this.controller = controller;
        this.cam = cam;
        this.controls = controls;
        timeTillNextFire = 0f;
        if (currentAmmo == -1)
            currentAmmo = maxAmmo;
    }

    /// <summary>
    /// Handles the action of switching to this weapon.
    /// </summary>
    /// <returns></returns>
    public IEnumerator Equip()
    {
        gameObject.SetActive(true);
        timeTillNextFire = 0f;
        currentState = States.Busy;

        yield return new WaitForSeconds(0.25f);

        currentState = States.Idle;
        controller.ShowReticle(true);
        controller.UpdateReticle(reticle, reticleSize);
        UpdateAmmoText();
    }

    /// <summary>
    /// Handles the action of switching away from this weapon.
    /// </summary>
    /// <returns></returns>
    public IEnumerator Unequip()
    {
        controller.ShowReticle(false);
        currentState = States.Busy;
        animator.SetTrigger("Unequip");
        yield return new WaitForSeconds(0.25f);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Handles the firing of the weapon.
    /// </summary>
    public abstract void Fire();

    protected abstract void UpdateAmmoText();
}