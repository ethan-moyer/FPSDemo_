using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.VFX;

public class PlayerCombatController : MonoBehaviour
{
    [Header("Weapons")]
    [SerializeField] private Transform weaponsContainer = null;
    [SerializeField] private int currentID = 0;
    [SerializeField] private int secondID = 1;
    [Header("Grenades")]
    [SerializeField] private GameObject fragPrefab = null;
    [SerializeField] private GameObject stickyPrefab = null;
    [SerializeField] private float grenadeThrowTime;
    private int currentGrenadeType = 0;
    private float grenadeCooldown;
    [SerializeField] private int currentFragAmount = 2;
    [SerializeField] private int currentStickyAmount = 0;
    [SerializeField] private int maxGrenadeAmount = 2;
    [SerializeField] private Vector3 grenadeSpawnOffset = Vector3.zero;
    [SerializeField] private float throwingForce = 20f;
    [Header("Models")]
    [SerializeField] private Transform viewModelPivot = null;
    [SerializeField] private GameObject viewModel = null;
    [SerializeField] private VisualEffect viewEffect = null;
    [SerializeField] private GameObject worldModel = null;
    [Header("HUD")]
    [SerializeField] private Image hudReticle = null;
    [SerializeField] private TextMeshProUGUI hudAmmo = null;
    [SerializeField] private Image hudCooldownMeter = null;
    [SerializeField] private Camera cam = null;
    [SerializeField] private float zoomTime = 1f;
    [SerializeField] private Image fragPanel = null;
    [SerializeField] private TextMeshProUGUI fragCounter = null;
    [SerializeField] private Image stickyPanel = null;
    [SerializeField] private TextMeshProUGUI stickyCounter = null;
    private float startingFOV = 0f;
    private MeshFilter viewModelFilter = null;
    private MeshRenderer viewModelRenderer = null;
    private Animator viewModelAnimator = null;
    private MeshFilter worldModelFilter = null;
    private MeshRenderer worldModelRenderer = null;
    private PlayerInputReader controls = null;
    private FirstPersonCamera firstPersonCamera = null;
    private VirtualAudioSource audioSource = null;
    private Dictionary<int, ModularWeapon> weapons;
    private int[] startingWeapons;

    public ModularWeapon CurrentWeapon { get; private set; }
    public ModularWeapon SecondWeapon
    {
        get { return weapons[secondID]; }
    }

    private void Awake()
    {
        startingFOV = cam.fieldOfView;
        viewModelFilter = viewModel.GetComponent<MeshFilter>();
        viewModelRenderer = viewModel.GetComponent<MeshRenderer>();
        viewModelAnimator = viewModel.GetComponent<Animator>();
        worldModelFilter = worldModel.GetComponent<MeshFilter>();
        worldModelRenderer = worldModel.GetComponent<MeshRenderer>();
        controls = GetComponent<PlayerInputReader>();
        firstPersonCamera = GetComponent<FirstPersonCamera>();
        audioSource = GetComponents<VirtualAudioSource>()[1];
        weapons = new Dictionary<int, ModularWeapon>();
        startingWeapons = new int[] { currentID, secondID };

        foreach (Transform t in weaponsContainer)
        {
            ModularWeapon w = t.GetComponent<ModularWeapon>();
            if (w != null)
            {
                w.Init(GetComponent<PlayerController>(), cam.transform, -1);
                w.TriggeringAnimation.AddListener(this.OnTriggerAnimation);
                w.UpdatingAmmoText.AddListener(this.UpdateAmmoText);
                w.ChangingFOV.AddListener(this.OnChangeFOV);
                w.PlayingAudioClip.AddListener(this.OnPlayAudioClip);
                w.PlayingEffect.AddListener(this.OnPlayEffect);
                if (w is ModularCooldownWeapon cooldown)
                    cooldown.UpdatingCooldownMeter.AddListener(this.OnUpdateCooldown);
                weapons.Add(w.WeaponID, w);
            }
        }

        UpdateGrenadeCounters();
        SwitchTo(currentID, -1);
    }

    public void ResetWeapons()
    {
        weapons[startingWeapons[0]].Init(GetComponent<PlayerController>(), cam.transform, -1);
        weapons[startingWeapons[1]].Init(GetComponent<PlayerController>(), cam.transform, -1);

        currentID = startingWeapons[0];
        secondID = startingWeapons[1];

        SwitchTo(currentID, -1);

        currentFragAmount = maxGrenadeAmount;
        currentStickyAmount = maxGrenadeAmount;
        grenadeCooldown = 0f;

        UpdateGrenadeCounters();
    }

    public void SwitchTo(int index, int ammo)
    {
        if (weapons.ContainsKey(index))
        {
            currentID = index;
            weapons[index].SetAmmo(ammo);
            StartCoroutine(weapons[index].Equip());
            CurrentWeapon = weapons[currentID];

            UpdateReticle();
            UpdateModels();
        }
    }

    public IEnumerator SwapWeapons()
    {
        hudReticle.gameObject.SetActive(false);
        yield return StartCoroutine(weapons[currentID].Unequip());

        CurrentWeapon = weapons[secondID];
        int tempID = currentID;
        currentID = secondID;
        secondID = tempID;

        UpdateReticle();
        UpdateModels();

        yield return StartCoroutine(weapons[currentID].Equip());
    }

    public void ShowViewModels(bool show)
    {
        if (show == true)
        {
            viewModelFilter.mesh = CurrentWeapon.ViewModel.mesh;
            viewEffect.gameObject.SetActive(true);
            viewEffect.Stop();
        }
        else
        {
            viewModelFilter.mesh = null;
            viewEffect.gameObject.SetActive(false);
        }
    }

    public void UpdateModels()
    {
        //View Model
        viewModelFilter.mesh = CurrentWeapon.ViewModel.mesh;
        viewModelRenderer.materials = CurrentWeapon.ViewModel.materials;
        viewModelAnimator.runtimeAnimatorController = CurrentWeapon.ViewModel.animator;
        viewModelPivot.localPosition = CurrentWeapon.ViewModel.offset;
        viewModelPivot.localScale = CurrentWeapon.ViewModel.scale;
        //View Model Visual Effect
        viewEffect.transform.localPosition = CurrentWeapon.ParticleEffect.offset;
        viewEffect.transform.localScale = CurrentWeapon.ParticleEffect.scale;
        viewEffect.visualEffectAsset = CurrentWeapon.ParticleEffect.visualEffect;
        //World Model
        worldModelFilter.mesh = CurrentWeapon.WorldModel.mesh;
        worldModelRenderer.materials = CurrentWeapon.WorldModel.materials;
        worldModel.transform.localPosition = CurrentWeapon.WorldModel.offset;
        worldModel.transform.localScale = CurrentWeapon.WorldModel.scale;
    }

    public void UpdateReticle()
    {
        hudReticle.gameObject.SetActive(true);
        hudReticle.sprite = CurrentWeapon.ReticleData.Item1;
        hudReticle.rectTransform.localScale = Vector3.one * CurrentWeapon.ReticleData.Item2;
        if (CurrentWeapon is ModularCooldownWeapon)
            hudCooldownMeter.gameObject.SetActive(true);
        else
            hudCooldownMeter.gameObject.SetActive(false);
    }

    public void UpdateAmmoText(string newText)
    {
        hudAmmo.text = newText;
    }

    private void UpdateGrenadeCounters()
    {
        if (currentGrenadeType == 0)
        {
            fragPanel.color = new Color(fragPanel.color.r, fragPanel.color.g, fragPanel.color.b, 0.59f);
            stickyPanel.color = new Color(stickyPanel.color.r, stickyPanel.color.g, stickyPanel.color.b, 0.39f);
        }
        else if (currentGrenadeType == 1)
        {
            fragPanel.color = new Color(fragPanel.color.r, fragPanel.color.g, fragPanel.color.b, 0.39f);
            stickyPanel.color = new Color(stickyPanel.color.r, stickyPanel.color.g, stickyPanel.color.b, 0.59f);
        }
        fragCounter.text = currentFragAmount.ToString();
        stickyCounter.text = currentStickyAmount.ToString();
    }

    private void OnTriggerAnimation(string parameter)
    {
        viewModelAnimator.SetTrigger(parameter);
    }

    public void SetStartingFOV(float newFOV)
    {
        startingFOV = newFOV;
    }

    private void OnChangeFOV(float fovMultiplier)
    {
        if (fovMultiplier == 1)
        {
            StartCoroutine(ChangeFOVOverTime(startingFOV));
            firstPersonCamera.zoomed = false;
            ShowViewModels(true);
        }
        else
        {
            StartCoroutine(ChangeFOVOverTime(startingFOV * fovMultiplier));
            firstPersonCamera.zoomed = true;
            ShowViewModels(false);
        }
    }

    private IEnumerator ChangeFOVOverTime(float newFOV)
    {
        float timer = 0f;
        while (timer < zoomTime)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newFOV, timer / zoomTime);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    private void OnPlayAudioClip(AudioClip clip)
    {
        audioSource.Play(clip);
    }

    private void OnPlayEffect()
    {
        viewEffect.Play();
    }

    private void OnUpdateCooldown(float meterAmount)
    {
        hudCooldownMeter.fillAmount = meterAmount;
    }

    public bool AddGrenade(int type)
    {
        if (type == 0 && currentFragAmount < maxGrenadeAmount)
        {
            currentFragAmount += 1;
            UpdateGrenadeCounters();
            return true;
        }
        else if (type == 1 && currentStickyAmount < maxGrenadeAmount)
        {
            currentStickyAmount += 1;
            UpdateGrenadeCounters();
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Update()
    {
        if (grenadeCooldown > 0f)
            grenadeCooldown -= Time.deltaTime;

        if (controls.WeaponSwitchDown && CurrentWeapon.CurrentState == ModularWeapon.States.Idle)
        {
            StartCoroutine(SwapWeapons());
        }
        if (controls.WeaponFireHeld)
        {
            CurrentWeapon.PrimaryAction();
        }
        if (controls.WeaponZoomDown)
        {
            CurrentWeapon.SecondaryAction();
        }
        if (controls.WeaponReloadDown)
        {
            CurrentWeapon.TertiaryAction();
        }
        if (controls.WeaponMeleeDown)
        {
            CurrentWeapon.MeleeAction();
        }
        if (controls.ThrowGrenadeDown)
        {
            if (CurrentWeapon.CurrentState == ModularWeapon.States.Idle && grenadeCooldown <= 0f)
            {
                if (currentGrenadeType == 0 && currentFragAmount > 0)
                {
                    currentFragAmount -= 1;
                    Instantiate(fragPrefab, cam.transform.TransformPoint(grenadeSpawnOffset), Quaternion.LookRotation(cam.transform.forward)).GetComponent<Grenade>().SetUp(GetComponent<PlayerController>(), cam.transform.forward * throwingForce);
                }
                else if (currentGrenadeType == 1 && currentStickyAmount > 0)
                {
                    currentStickyAmount -= 1;
                    Instantiate(stickyPrefab, cam.transform.TransformPoint(grenadeSpawnOffset), Quaternion.LookRotation(cam.transform.forward)).GetComponent<Grenade>().SetUp(GetComponent<PlayerController>(), cam.transform.forward * throwingForce);
                }
                grenadeCooldown = grenadeThrowTime;
                UpdateGrenadeCounters();
            }
        }
        if (controls.SwitchGrenadeDown)
        {
            if (currentGrenadeType == 0)
                currentGrenadeType = 1;
            else if (currentGrenadeType == 1)
                currentGrenadeType = 0;
            UpdateGrenadeCounters();
        }
    }
}
