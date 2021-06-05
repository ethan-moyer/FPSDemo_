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
    [Header("Models")]
    [SerializeField] private Transform viewModelPivot = null;
    [SerializeField] private GameObject viewModel = null;
    [SerializeField] private VisualEffect viewEffect = null;
    [SerializeField] private GameObject worldModel = null;
    [Header("HUD")]
    [SerializeField] private Image hudReticle = null;
    [SerializeField] private TextMeshProUGUI hudAmmo = null;
    [SerializeField] private Camera cam = null;
    [SerializeField] private float zoomTime = 1f;
    private float startingFOV = 0f;
    private MeshFilter viewModelFilter = null;
    private MeshRenderer viewModelRenderer = null;
    private Animator viewModelAnimator = null;
    private MeshFilter worldModelFilter = null;
    private MeshRenderer worldModelRenderer = null;
    private PlayerInputReader controls;
    private Dictionary<int, Weapon> weapons;
    private Weapon currentWeapon;

    public Weapon CurrentWeapon
    {
        get { return currentWeapon; }
    }

    public Weapon SecondWeapon
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
        weapons = new Dictionary<int, Weapon>();

        foreach (Transform t in weaponsContainer)
        {
            Weapon w = t.GetComponent<Weapon>();
            if (w != null)
            {
                w.Init(this, cam.transform, controls, viewModelAnimator, viewEffect);
                w.triggerAnimation.AddListener(this.OnTriggerAnimation);
                w.updateAmmoText.AddListener(this.UpdateAmmoText);
                w.changeFOV.AddListener(this.OnChangeFOV);
                weapons.Add(w.WeaponID, w);
            }
        }

        SwitchTo(currentID, -1);
    }

    public void SwitchTo(int index, int ammo)
    {
        if (weapons.ContainsKey(index))
        {
            currentID = index;
            weapons[index].SetAmmo(ammo);
            StartCoroutine(weapons[index].Equip());
            currentWeapon = weapons[currentID];

            UpdateReticle();
            UpdateModels();
        }
    }

    public IEnumerator SwapWeapons()
    {
        hudReticle.gameObject.SetActive(false);
        yield return StartCoroutine(weapons[currentID].Unequip());

        currentWeapon = weapons[secondID];
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
            viewModelFilter.mesh = currentWeapon.ViewModel.mesh;
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
        viewModelFilter.mesh = currentWeapon.ViewModel.mesh;
        viewModelRenderer.materials = currentWeapon.ViewModel.materials;
        viewModelAnimator.runtimeAnimatorController = currentWeapon.ViewModel.animator;
        viewModelPivot.localPosition = currentWeapon.ViewModel.offset;
        viewModelPivot.localScale = currentWeapon.ViewModel.scale;
        //View Model Visual Effect
        viewEffect.transform.localPosition = currentWeapon.ParticleEffect.offset;
        viewEffect.transform.localScale = currentWeapon.ParticleEffect.scale;
        viewEffect.visualEffectAsset = currentWeapon.ParticleEffect.visualEffect;
        //World Model
        worldModelFilter.mesh = currentWeapon.WorldModel.mesh;
        worldModelRenderer.materials = currentWeapon.WorldModel.materials;
        worldModel.transform.localPosition = currentWeapon.WorldModel.offset;
        worldModel.transform.localScale = currentWeapon.WorldModel.scale;
    }

    public void UpdateReticle()
    {
        hudReticle.gameObject.SetActive(true);
        hudReticle.sprite = currentWeapon.ReticleData.Item1;
        hudReticle.rectTransform.localScale = Vector3.one * currentWeapon.ReticleData.Item2;
    }

    public void UpdateAmmoText(string newText)
    {
        hudAmmo.text = newText;
    }

    private void OnTriggerAnimation(string parameter)
    {
        viewModelAnimator.SetTrigger(parameter);
    }

    private void OnChangeFOV(float fovMultiplier)
    {
        if (fovMultiplier == -1)
        {
            StartCoroutine(ChangeFOVOverTime(startingFOV));
            ShowViewModels(true);
        }
        else
        {
            StartCoroutine(ChangeFOVOverTime(startingFOV * fovMultiplier));
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

    private void Update()
    {
        if (controls.WeaponSwitchDown && currentWeapon.IsIdle)
        {
            StartCoroutine(SwapWeapons());
        }
        if (controls.WeaponFireHeld)
        {
            currentWeapon.PrimaryAction();
        }
        if (controls.WeaponZoomDown)
        {
            currentWeapon.SecondaryAction();
        }
        if (controls.WeaponReloadDown)
        {
            currentWeapon.ThirdAction();
        }
    }
}
