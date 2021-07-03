using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public abstract class Weapon : MonoBehaviour
{
    public StringEvent TriggerAnimation;
    public StringEvent UpdateAmmoText;
    public FloatEvent ChangeFOV;
    public AudioEvent PlayAudioClip;
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
    [SerializeField] protected bool stayZoomed = false;
    [SerializeField] protected float zoomFOVMultiplier = 0.5f;
    [Header("Attack Attributes")]
    [SerializeField] protected float HPDamage = 0;
    [SerializeField] protected float SPDamage = 0;
    [SerializeField] protected float fireRate = 0f;
    [SerializeField] protected int maxAmmo = 100;
    protected bool isZoomed = false;
    protected int currentAmmo = 0;

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

    public bool AddAmmo(int additionalAmmo)
    {
        if (CurrentAmmo == maxAmmo)
        {
            return false;
        }
        else
        {
            currentAmmo = Mathf.Min(currentAmmo + additionalAmmo, maxAmmo);
            return true;
        }
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
        UpdateAmmoText.Invoke(AmmoToText());

        yield return new WaitForSeconds(0.25f);

        currentState = States.Idle;
    }

    public virtual IEnumerator Unequip()
    {
        currentState = States.Busy;
        TriggerAnimation.Invoke("Unequip");

        yield return new WaitForSeconds(0.25f);

        gameObject.SetActive(false);
    }
    
    protected GameObject PlaceEffect(int index, Vector3 position, Vector3 normal)
    {
        GameObject hitEffect = ObjectPooler.SharedInstance.GetPooledObject(index);
        if (hitEffect != null)
        {
            hitEffect.transform.position = position;
            hitEffect.transform.rotation = Quaternion.LookRotation(normal);
            hitEffect.SetActive(true);
            return hitEffect;
        }
        return null;
    }

    protected Vector3 ShootRay(Vector3 direction)
    {
        RaycastHit hit;
        combatController.gameObject.layer = 2;
        if (Physics.Raycast(cam.position, direction, out hit, maxDistance))
        {
            if (hit.transform.gameObject.layer == 9)
            {
                //Hit a Player.
                PlayerController player = hit.transform.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.DamageHit(HPDamage, SPDamage);
                }
            }
            if (hit.transform.gameObject.layer == 12)
            {
                //Hit a Pickup or Prop.
                PlaceEffect(1, hit.point, hit.normal);
                Prop prop = hit.transform.GetComponent<Prop>();
                if (prop != null)
                {
                    prop.Hit(hit.point, cam.forward * 10f);
                }
            }
            else if (hit.transform.gameObject.layer == 10)
            {
                //Hit Terrain.
                PlaceEffect(0, hit.point, hit.normal);
            }
            return hit.point;
        }
        combatController.gameObject.layer = 9;
        return cam.position + direction * maxDistance;
    }

    public abstract void PrimaryAction();

    public abstract void SecondaryAction();

    public abstract void ThirdAction();

    public abstract string AmmoToText();
}
