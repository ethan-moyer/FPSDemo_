using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon Identification")]
    [SerializeField] protected int weaponID = 0;
    [SerializeField] protected string weaponName = "";
    [Header("Models")]
    [SerializeField] protected WeaponModel viewModel;
    [SerializeField] protected WeaponModel worldModel;
    [SerializeField] protected WeaponEffect particleEffect;
    [SerializeField] protected GameObject propPrefab;
    [Header("Aiming")]
    [SerializeField] private Sprite reticle = null;
    [SerializeField] private float reticleScale = 0.1f;
    [SerializeField] protected float maxDistance = 80f;
    [SerializeField] protected float coneRadius = 5f;
    [Header("Attack Attributes")]
    [SerializeField] protected float fireRate = 0f;
    [SerializeField] protected bool stayZoomed = false;
    protected bool isZoomed = false;
    protected int currentAmmo = 0;
    [SerializeField] protected int maxAmmo = 100;

    protected Animator animator = null;
    protected VisualEffect effect = null;
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

    public virtual int CurrentAmmo
    {
        get { return currentAmmo; }
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

    public GameObject PropPrefab
    {
        get { return propPrefab; }
    }

    public (Sprite, float) ReticleData
    {
        get { return (reticle, reticleScale); }
    }

    public void Init(PlayerCombatController combatController, Transform cam, PlayerInputReader controls, Animator animator, VisualEffect effect, int startingAmmo = -1)
    {
        this.combatController = combatController;
        this.cam = cam;
        this.controls = controls;
        this.animator = animator;
        this.effect = effect;
        SetAmmo(startingAmmo);
    }

    public void AddAmmo(int additionalAmmo)
    {
        currentAmmo = Mathf.Min(currentAmmo + additionalAmmo, maxAmmo);
    }

    public virtual void SetAmmo(int newAmmo)
    {
        if (newAmmo == -1)
            currentAmmo = maxAmmo;
        else
            currentAmmo = Mathf.Min(newAmmo, maxAmmo);
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

    public virtual IEnumerator Unequip()
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

    protected void PlaceHitEffect(int index, Vector3 position, Vector3 normal)
    {
        GameObject hitEffect = ObjectPooler.SharedInstance.GetPooledObject(index);
        if (hitEffect != null)
        {
            hitEffect.transform.position = position + normal*0.1f;
            hitEffect.transform.rotation = Quaternion.Euler(normal);
            hitEffect.SetActive(true);
        }
    }

    public abstract void PrimaryAction();

    public abstract void SecondaryAction();

    public abstract void ThirdAction();

    public abstract string AmmoToText();
}
