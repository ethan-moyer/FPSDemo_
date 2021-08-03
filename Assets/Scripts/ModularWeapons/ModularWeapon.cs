using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ModularWeapon : MonoBehaviour
{
    //Events
    public StringEvent TriggeringAnimation;
    public StringEvent UpdatingAmmoText;
    public FloatEvent ChangingFOV;
    public AudioEvent PlayingAudioClip;
    public UnityEvent PlayingEffect;

    //Attributes
    [Header("Weapon Identification")]
    [SerializeField] private string weaponName = "";
    [SerializeField] private int weaponID = 0;
    [Header("Models")]
    [SerializeField] private WeaponModel viewModel;
    [SerializeField] private WeaponModel worldModel;
    [SerializeField] private WeaponEffect particleEffect;
    [SerializeField] private GameObject propPrefab;
    [Header("HUD")]
    [SerializeField] private Sprite reticle = null;
    [SerializeField] private float reticleScale = 0.1f;
    [Header("Attack")]
    [SerializeField] public float maxDistance = 100f;
    [SerializeField] public float coneRadius = 5f;
    [SerializeField] protected int maxAmmo = 100;
    [SerializeField] protected float attackRate = 1f;
    [SerializeField, SerializeReference] protected WeaponAttack attack = null;
    [Header("FOV")]
    [SerializeField] protected float[] zoomLevels;
    [SerializeField] protected bool stayZoomed = false;

    protected Transform cam = null;
    protected PlayerController player = null;
    protected float attackTimer = 0f;
    protected int currentAmmo = 0;
    protected int currentZoomLevel = 0;
    private float defaultRadius;

    public enum States { Idle, Busy, Firing };
    public States CurrentState { get; set; }

    //Getters
    public int WeaponID
    {
        get { return weaponID; }
    }

    public virtual int TotalAmmo
    {
        get { return currentAmmo; }
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

    public Transform Cam => cam;

    //Methods
    public virtual void Init(PlayerController player, Transform cam, int ammo)
    {
        this.player = player;
        this.cam = cam;
        defaultRadius = coneRadius;
        SetAmmo(ammo);
    }

    public bool AddAmmo(int additionalAmmo)
    {
        if (currentAmmo >= maxAmmo)
        {
            return false;
        }
        else
        {
            currentAmmo = Mathf.Min(currentAmmo + additionalAmmo, maxAmmo);
            return true;
        }
    }

    public IEnumerator Equip()
    {
        this.gameObject.SetActive(true);
        attackTimer = 0f;
        CurrentState = States.Busy;
        UpdateAmmoText();

        yield return new WaitForSeconds(0.25f);

        CurrentState = States.Idle;
    }

    public IEnumerator Unequip()
    {
        CurrentState = States.Busy;
        SetZoomLevel(0);
        TriggeringAnimation.Invoke("Unequip");

        yield return new WaitForSeconds(0.25f);

        this.gameObject.SetActive(false);
    }

    protected void SetZoomLevel(int level)
    {
        if (zoomLevels.Length > 0)
        {
            currentZoomLevel = level % zoomLevels.Length;
            coneRadius = defaultRadius * zoomLevels[currentZoomLevel];
            ChangingFOV.Invoke(zoomLevels[currentZoomLevel]);
        }
    }

    public static void PlaceEffect(int index, Vector3 position, Vector3 normal)
    {
        GameObject effect = ObjectPooler.SharedInstance.GetPooledObject(index);
        if (effect != null)
        {
            effect.transform.position = position;
            effect.transform.rotation = Quaternion.LookRotation(normal);
            effect.SetActive(true);
        }
    }

    protected virtual void Update()
    {
        if (CurrentState == States.Firing)
        {
            if (attackTimer > 0f)
            {
                attackTimer -= Time.deltaTime;
            }
            else
            {
                attackTimer = 0f;
                CurrentState = States.Idle;
            }
        }
    }

    public abstract void SetAmmo(int ammo);

    public abstract void UpdateAmmoText();

    public abstract void PrimaryAction();

    public abstract void SecondaryAction();

    public abstract void TertiaryAction();
}
