using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct WeaponModel
{
    public Mesh model;
    public Material[] materials;
    public RuntimeAnimatorController animator;
    public Vector3 offset;
    public Vector3 scale;
}

public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon Identification")]
    [SerializeField] protected string weaponName = "Weapon";
    [SerializeField] protected int weaponID = 0;
    [Header("Models")]
    [SerializeField] protected WeaponModel viewModel;
    [SerializeField] protected WeaponModel worldModel;
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
    protected PlayerInputReader controls;
    protected Transform cam;
    protected float timeTillNextFire = 0f;
    protected enum States { Idle, Busy, Firing };
    protected States currentState;

    public int WeaponID => weaponID;
    public bool IsIdle => currentState == States.Idle;
    public float MaxAngle { get { return Vector3.Angle(cam.forward.normalized, cam.forward.normalized + cam.right.normalized); } }

    /// <summary>
    /// Handles tying this weapon to a player.
    /// </summary>
    /// <param name="controller">The PlayerCombatController to link it to.</param>
    /// <param name="cam">The camera of the player to link it to.</param>
    /// <param name="controls">The PlayerInputReader to link it to.</param>
    public virtual void SetUp(PlayerCombatController controller, Transform cam, PlayerInputReader controls, Animator animator)
    {
        this.controller = controller;
        this.cam = cam;
        this.controls = controls;
        this.animator = animator;
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
        float timeStart = Time.time;
        gameObject.SetActive(true);
        timeTillNextFire = 0f;
        currentState = States.Busy;
        controller.UpdateModel(worldModel, true);
        controller.UpdateModel(viewModel, false);

        yield return new WaitForSeconds(0.25f);

        currentState = States.Idle;
        controller.ShowReticle(true);
        controller.UpdateReticle(reticle, reticleSize);
        UpdateAmmoText();
        Debug.Log(Time.time - timeStart);
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
        Debug.Log("Unequip time up.");
    }

    /// <summary>
    /// Handles the action corresponding to the left-mouse/right-trigger.
    /// </summary>
    public abstract void PrimaryAction();

    /// <summary>
    /// Handles the action corresponding to the right-mouse/left-trigger.
    /// </summary>
    public abstract void SecondaryAction();

    /// <summary>
    /// Handles the action corresponding to the R-key/west button.
    /// </summary>
    public abstract void ReloadAction();

    protected abstract void UpdateAmmoText();
}