using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon Identification")]
    [SerializeField] protected int weaponID = 0;
    [SerializeField] protected string weaponName = "";
    [Header("Models")]
    [SerializeField] protected WeaponModel viewModel;
    [SerializeField] protected WeaponModel worldModel;
    [SerializeField] protected WeaponEffect particleEffect;
    [Header("Aiming")]
    [SerializeField] private Sprite reticle = null;
    [SerializeField] private float reticleScale = 0.1f;
    [SerializeField] protected float maxDistance = 80f;
    [SerializeField] protected float coneRadius = 5f;
    [Header("Attack Attributes")]
    [SerializeField] protected float damage = 0f;
    [SerializeField] protected float fireRate = 0f;
    [SerializeField] protected int currentAmmo = -1;
    [SerializeField] protected int maxAmmo = 100;
    protected Animator animator = null;
    protected Transform cam = null;
    protected PlayerCombatController combatController = null;
    protected PlayerInputReader controls = null;
    protected enum States { Idle, Busy, Firing };
    protected States currentState;
    protected float timeTillNextFire = 0f;

    public int WeaponID
    {
        get { return weaponID; }
    }

    public bool IsIdle
    {
        get { return currentState == States.Idle; }
    }

    public WeaponModel ViewModel
    {
        get { return viewModel; }
    }

    public WeaponModel WorldModel
    {
        get { return worldModel; }
    }

    public WeaponEffect ParticleEffect
    {
        get { return particleEffect; }
    }

    public (Sprite, float) ReticleData
    {
        get { return (reticle, reticleScale); }
    }

    public virtual void Init(PlayerCombatController combatController, Transform cam, PlayerInputReader controls, Animator animator)
    {
        this.combatController = combatController;
        this.cam = cam;
        this.controls = controls;
        this.animator = animator;
        AddAmmo(currentAmmo);
    }

    public void AddAmmo(int newAmmo)
    {
        if (newAmmo == -1)
            currentAmmo = maxAmmo;
        else
            currentAmmo = Mathf.Min(currentAmmo + newAmmo, maxAmmo);
    }

    public IEnumerator Equip()
    {
        gameObject.SetActive(true);
        timeTillNextFire = 0f;
        currentState = States.Busy;
        combatController.UpdateAmmoText(AmmoToText());

        yield return new WaitForSeconds(0.25f);

        currentState = States.Idle;
    }

    public IEnumerator Unequip()
    {
        currentState = States.Busy;
        animator.SetTrigger("Unequip");

        yield return new WaitForSeconds(0.25f);

        gameObject.SetActive(false);
    }
    
    protected void PlaceEffect(int index, Vector3 position, Vector3 normal)
    {
        GameObject hitEffect = ObjectPooler.SharedInstance.GetPooledObject(index);
        if (hitEffect != null)
        {
            hitEffect.transform.position = position;
            hitEffect.transform.rotation = Quaternion.LookRotation(normal);
            hitEffect.SetActive(true);
        }
    }

    public abstract void PrimaryAction();

    public abstract void SecondaryAction();

    public abstract void ThirdAction();

    public abstract string AmmoToText();
}
