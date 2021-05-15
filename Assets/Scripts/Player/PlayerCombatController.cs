using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// This component handles the combat aspects for playable characters.
/// This includes: managing weapons, handling combat input (shooting, reloading, etc.), player health, and updating the HUD.
/// This component should be added to the root player prefab along with PlayerInput, PlayerMomvementController, PlayerInputReader, and FirstPersonCamera.
/// </summary>
public class PlayerCombatController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform viewModelPivot;
    [SerializeField] private GameObject viewModel;
    [SerializeField] private Animator viewModelAnimator;
    [SerializeField] private GameObject worldModel;
    [SerializeField] private Image hudReticle;
    [SerializeField] private TextMeshProUGUI hudAmmo;
    [SerializeField] private Transform cam;
    [SerializeField] private Transform weaponsContainer;
    [SerializeField] private int currentID;
    [SerializeField] private int secondID;

    private PlayerInputReader controls;
    private Dictionary<int, Weapon> weapons;
    private Weapon currentWeapon;

    private void Awake()
    {
        controls = GetComponent<PlayerInputReader>();
        weapons = new Dictionary<int, Weapon>();
        foreach (Transform t in weaponsContainer)
        {
            Weapon w = t.GetComponent<Weapon>();
            if (w != null)
            {
                w.SetUp(this, cam, controls, viewModelAnimator);
                weapons.Add(w.WeaponID, w);
            }
        }
        if (weapons.ContainsKey(currentID) && weapons.ContainsKey(secondID))
        {
            StartCoroutine(weapons[currentID].Equip());
            currentWeapon = weapons[currentID];
        }
    }

    private IEnumerator SwitchWeapon()
    {
        hudReticle.gameObject.SetActive(false);
        yield return StartCoroutine(weapons[currentID].Unequip());
        yield return StartCoroutine(weapons[secondID].Equip());
        currentWeapon = weapons[secondID];

        int tempID = currentID;
        currentID = secondID;
        secondID = tempID;
    }

    public void ShowReticle(bool shouldShow)
    {
        hudReticle.gameObject.SetActive(shouldShow);
    }

    public void UpdateReticle(Sprite newReticle, float newSize)
    {
        hudReticle.gameObject.SetActive(true);
        hudReticle.sprite = newReticle;
        hudReticle.rectTransform.localScale = Vector3.one * newSize;
    }

    public void UpdateAmmoText(string newText)
    {
        hudAmmo.text = newText;
    }

    public void UpdateModel(WeaponModel newModel, bool updatingWorldModel)
    {
        if (updatingWorldModel == true)
        {
            worldModel.GetComponent<MeshFilter>().mesh = newModel.model;
            worldModel.GetComponent<MeshRenderer>().materials = newModel.materials;
            worldModel.transform.localPosition = newModel.offset;
            worldModel.transform.localScale = newModel.scale;
        }
        else
        {
            viewModel.GetComponent<MeshFilter>().mesh = newModel.model;
            viewModel.GetComponent<MeshRenderer>().materials = newModel.materials;
            viewModelAnimator.runtimeAnimatorController = newModel.animator;
            viewModelPivot.localPosition = newModel.offset;
            viewModelPivot.localScale = newModel.scale;
        }
    }

    private void Update()
    {
        //Input
        if (controls.WeaponSwitchDown && currentWeapon.IsIdle)
        {
            StartCoroutine(SwitchWeapon());
        }
        if (controls.WeaponFireHeld)
        {
            currentWeapon.PrimaryAction();
        }
        if (controls.WeaponReloadDown)
        {
            currentWeapon.ReloadAction();
        }

        //Objects within reticle
        RaycastHit hit;
        gameObject.layer = 2;
        if (currentWeapon != null && Physics.SphereCast(cam.position, currentWeapon.reticleSize, cam.forward, out hit, currentWeapon.maxRange))
        {
            if (hit.transform.gameObject.layer == 12)
                hudReticle.color = Color.red;
            else
                hudReticle.color = Color.white;
        }
        else
        {
            hudReticle.color = Color.white;
        }
        gameObject.layer = 9;
    }
}
