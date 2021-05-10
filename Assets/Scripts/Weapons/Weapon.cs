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

    public int WeaponID => weaponID;

    public float MaxAngle { get { return Vector3.Angle(cam.forward.normalized, cam.forward.normalized + cam.right.normalized); } }

    public abstract bool CanSwitch { get; }

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetUp(PlayerCombatController controller, Transform cam, PlayerInputManager controls)
    {
        this.controller = controller;
        this.cam = cam;
        this.controls = controls;
        timeTillNextFire = 0f;
        if (currentAmmo == -1)
            currentAmmo = maxAmmo;
    }

    //When you switch to this weapon.
    public abstract IEnumerator Equip();

    //When you switch away from this weapon.
    public abstract IEnumerator Unequip();

    //When the weapon is fired.
    public abstract void Fire();
}