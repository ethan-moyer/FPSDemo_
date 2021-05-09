using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerCombatController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Image hudReticle;
    [SerializeField] private Transform cam;
    [SerializeField] private Transform weaponsContainer;
    [SerializeField] private int currentID;
    [SerializeField] private int secondID;
    private PlayerInputManager controls;
    private Dictionary<int, Weapon> weapons;
    private Weapon currentWeapon;

    private void Awake()
    {
        controls = GetComponent<PlayerInputManager>();
        weapons = new Dictionary<int, Weapon>();
        foreach (Transform t in weaponsContainer)
        {
            Weapon w = t.GetComponent<Weapon>();
            if (w != null)
            {
                w.SetUp(this, cam, controls);
                weapons.Add(w.WeaponID, w);
            }
        }
        if (weapons.ContainsKey(currentID) && weapons.ContainsKey(secondID))
        {
            StartCoroutine(weapons[currentID].Equip());
            currentWeapon = weapons[currentID];
            UpdateReticle();
        }
    }

    private IEnumerator SwitchWeapon()
    {
        hudReticle.gameObject.SetActive(false);
        yield return StartCoroutine(weapons[currentID].Unequip());
        yield return StartCoroutine(weapons[secondID].Equip());
        currentWeapon = weapons[secondID];
        UpdateReticle();

        int tempID = currentID;
        currentID = secondID;
        secondID = tempID;
    }

    private void UpdateReticle()
    {
        hudReticle.gameObject.SetActive(true);
        hudReticle.sprite = currentWeapon.reticle;
        hudReticle.rectTransform.localScale = Vector3.one * currentWeapon.reticleSize;
    }

    private void Update()
    {
        if (controls.WeaponSwitchDown && currentWeapon.CanSwitch)
        {
            StartCoroutine(SwitchWeapon());
        }

        if (controls.WeaponFireHeld)
        {
            currentWeapon.Fire();
        }

        RaycastHit hit;
        gameObject.layer = 2;
        if (currentWeapon != null && Physics.SphereCast(cam.position, currentWeapon.reticleSize, cam.forward, out hit))
        {
            Debug.Log(hit.transform.name);
            hudReticle.color = Color.red;
        }
        else
        {
            hudReticle.color = Color.white;
        }
    }

    private void OnDrawGizmos()
    {
        if (currentWeapon != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(cam.position + cam.forward, currentWeapon.reticleSize);
        }
    }
}
