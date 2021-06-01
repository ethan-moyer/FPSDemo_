using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attached to a player and is responsible for player methods that don't fall under CombatController or MovementController.
/// Things like interaction with multiplayer game manager, interacting with objects in the world, etc.
/// </summary>
[RequireComponent(typeof(PlayerCombatController))]
[RequireComponent(typeof(PlayerMovementController))]
[RequireComponent(typeof(PlayerInputReader))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform cam = null;
    [SerializeField] private LayerMask defaultCullingMask;
    [SerializeField] private Transform viewModels = null;
    [SerializeField] private Transform worldModels = null;
    private PlayerCombatController combatController = null;
    private PlayerMovementController movementController = null;
    private PlayerInputReader controls = null;
    private CharacterController cc = null;

    private void Awake()
    {
        combatController = GetComponent<PlayerCombatController>();
        movementController = GetComponent<PlayerMovementController>();
        controls = GetComponent<PlayerInputReader>();
        cc = GetComponent<CharacterController>();
    }

    /// <summary>
    /// Updates the camera culling mask and the layers of the weapon models to work for splitscreen.
    /// </summary>
    /// <param name="playerIndex"></param>
    public void AssignLayers(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex > 3)
        {
            Debug.LogError($"{playerIndex} is an invalid index. Player index must be between [0, 3].");
            return;
        }

        int worldLayer = 17 + playerIndex;
        int viewLayer = 13 + playerIndex;
        Camera camera = cam.GetComponent<Camera>();
        camera.cullingMask = defaultCullingMask;
        camera.cullingMask |= 1 << viewLayer;
        camera.cullingMask &= ~(1 << worldLayer);
        ChangeLayerRecursive(worldModels, worldLayer);
        ChangeLayerRecursive(viewModels, viewLayer);
    }

    public void PhysicsHit(Vector3 direction, float strength)
    {
        movementController.MoveDirection += direction * strength;
    }

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, 3f))
        {
            if (hit.transform.gameObject.layer == 11)
            {
                PropWeapon prop = hit.transform.GetComponent<PropWeapon>();
                if (prop != null)
                {
                    if (controls.WeaponInteractDown)
                    {
                        Destroy(prop.gameObject);
                        GameObject newProp = Instantiate(combatController.CurrentWeapon.PropPrefab, transform.position, combatController.CurrentWeapon.PropPrefab.transform.rotation);
                        newProp.GetComponent<PropWeapon>().ammo = combatController.CurrentWeapon.CurrentAmmo;
                        newProp.GetComponent<Rigidbody>().AddForce(cam.forward * 500f);
                        combatController.SwitchTo(prop.WeaponID, prop.Ammo);
                    }
                }
            }
        }
    }

    private void ChangeLayerRecursive(Transform obj, int newLayer)
    {
        obj.gameObject.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            ChangeLayerRecursive(child, newLayer);
        }
    }
}
