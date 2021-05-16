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
    [SerializeField] private Transform cam = null;
    private MeshFilter viewModelFilter = null;
    private MeshRenderer viewModelRenderer = null;
    private Animator viewModelAnimator = null;
    private PlayerInputReader controls;
    private Dictionary<int, Weapon> weapons;
    private Weapon currentWeapon;

    private void Awake()
    {
        viewModelFilter = viewModel.GetComponent<MeshFilter>();
        viewModelRenderer = viewModel.GetComponent<MeshRenderer>();
        viewModelAnimator = viewModel.GetComponent<Animator>();
        controls = GetComponent<PlayerInputReader>();
        weapons = new Dictionary<int, Weapon>();

        foreach (Transform t in weaponsContainer)
        {
            Weapon w = t.GetComponent<Weapon>();
            if (w != null)
            {
                w.Init(this, cam, controls, viewModelAnimator);
                weapons.Add(w.WeaponID, w);
            }
        }

        SwitchTo(currentID);
    }

    public void SwitchTo(int index)
    {
        if (weapons.ContainsKey(index))
        {
            currentID = index;
            StartCoroutine(weapons[currentID].Equip());
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

    public void UpdateModels()
    {
        //View Model
        viewModelFilter.mesh = currentWeapon.ViewModel.mesh;
        viewModelRenderer.materials = currentWeapon.ViewModel.materials;
        viewModelAnimator.runtimeAnimatorController = currentWeapon.ViewModel.animator;
        viewModelPivot.localPosition = currentWeapon.ViewModel.offset;
        viewModelPivot.localScale = currentWeapon.ViewModel.scale;
        viewEffect.transform.localPosition = currentWeapon.ParticleEffect.offset;
        viewEffect.transform.localScale = currentWeapon.ParticleEffect.scale;
        viewEffect.visualEffectAsset = currentWeapon.ParticleEffect.visualEffect;
        
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
        if (controls.WeaponReloadDown)
        {
            currentWeapon.ThirdAction();
        }
    }
}
