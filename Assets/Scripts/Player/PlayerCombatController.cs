using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombatController : MonoBehaviour
{
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
        }
    }

    private IEnumerator SwitchWeapon()
    {
        yield return StartCoroutine(weapons[currentID].Unequip());
        yield return StartCoroutine(weapons[secondID].Equip());
        currentWeapon = weapons[secondID];

        int tempID = currentID;
        currentID = secondID;
        secondID = tempID;
    }

    private void Update()
    {
        if (controls.WeaponSwitchDown && currentWeapon.CanSwitch)
        {
            Debug.Log("Switching");
            StartCoroutine(SwitchWeapon());
        }

        if (controls.WeaponFireHeld)
        {
            currentWeapon.Fire();
        }
    }
}
