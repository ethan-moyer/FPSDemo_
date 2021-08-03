using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    public int PlayerID { get; set; }
    public IntIntEvent Die;
    [SerializeField] private Transform cam = null;
    [SerializeField] private LayerMask defaultCullingMask;
    [SerializeField] private Transform viewModels = null;
    [SerializeField] private Transform worldModels = null;
    [SerializeField] private GameObject hudCanvas = null;
    [SerializeField] private GameObject respawnCanvas = null;
    [SerializeField] private Image hudHealthBar = null;
    [SerializeField] private Image hudShieldBar = null;
    [SerializeField] private Image[] damageIndicators = new Image[4];
    [SerializeField] private TextMeshProUGUI score = null;
    [SerializeField] private GameObject shieldObject = null;
    [Header("Hit Points")]
    [SerializeField] private int maxHP = 75;
    [SerializeField] private int maxSP = 75;
    [SerializeField] private float HPRegenDelay = 10f;
    [SerializeField] private float HPRegenRate = 9;
    [SerializeField] private float SPRegenDelay = 5f;
    [SerializeField] private float SPRegenRate = 50;
    private PlayerCombatController combatController = null;
    private PlayerMovementController movementController = null;
    private PlayerInputReader controls = null;
    private CharacterController cc = null;
    private float currentHP;
    private float currentSP;
    private float HPRegenTimer = 0f;
    private float SPRegenTimer = 0f;

    private void Awake()
    {
        combatController = GetComponent<PlayerCombatController>();
        movementController = GetComponent<PlayerMovementController>();
        controls = GetComponent<PlayerInputReader>();
        cc = GetComponent<CharacterController>();

        currentHP = maxHP;
        currentSP = maxSP;

        foreach (Image i in damageIndicators)
        {
            i.canvasRenderer.SetAlpha(0f);
        }
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

    public void UpdateCanvasScale()
    {
        CanvasScaler canvasScaler = hudCanvas.GetComponent<CanvasScaler>();
        Camera camera = cam.GetComponent<Camera>();
        canvasScaler.referenceResolution = new Vector2(1920 / camera.rect.width, 1080 / camera.rect.width);
    }

    public void UpdateScoreText(string newScore)
    {
        score.text = $"Score:\n{newScore}";
    }

    public void PhysicsHit(Vector3 direction, float strength)
    {
        movementController.MoveDirection += direction * strength;
    }

    public void DamageHit(float HPDamage, float SPMultiplier, Vector3 hitPos, PlayerController otherPlayer)
    {
        //Apply Damage
        float remainingDamage = HPDamage;
        if (currentSP > 0)
        {
            remainingDamage = Mathf.Max(0, (HPDamage * SPMultiplier) - currentSP);
            currentSP = Mathf.Max(0, currentSP - (HPDamage * SPMultiplier));
            SPRegenTimer = 0f;
        }
        
        if (remainingDamage > 0)
        {
            currentHP -= remainingDamage;
            if (currentHP <= 0)
            {
                Die.Invoke(PlayerID, otherPlayer.PlayerID);
                GameObject effect = ObjectPooler.SharedInstance.GetPooledObject(5);
                effect.transform.position = transform.position;
                effect.SetActive(true);
            }
            else
            {
                HPRegenTimer = 0f;
            }
        }
        hudHealthBar.fillAmount = currentHP / maxHP;
        hudShieldBar.fillAmount = currentSP / maxSP;

        //Show Hit Indicator
        Vector3 hitDir = (hitPos - transform.position); hitDir.y = 0f; hitDir.Normalize();
        float damageDot = Vector3.Dot(transform.forward, hitDir);
        int indicatorIndex;
        if (damageDot >= 0.5f)
        {
            indicatorIndex = 0;
        }
        else if (damageDot <= -0.5f)
        {
            indicatorIndex = 2;
        }
        else
        {
            Vector3 perpendicular = Vector3.Cross(transform.forward, hitDir);
            if (Vector3.Dot(perpendicular, Vector3.up) > 0f)
            {
                indicatorIndex = 1;
            }
            else
            {
                indicatorIndex = 3;
            }
        }
        damageIndicators[indicatorIndex].canvasRenderer.SetAlpha(1f);
        damageIndicators[indicatorIndex].CrossFadeAlpha(0f, 1f, false);

        shieldObject.SetActive(true);
    }

    public void Respawn(Vector3 spawnPoint, Vector3 newRotation)
    {
        Enable();
        currentHP = maxHP;
        currentSP = maxSP;
        hudHealthBar.fillAmount = 1;
        hudShieldBar.fillAmount = 1;
        transform.rotation = Quaternion.Euler(newRotation);
        movementController.MoveDirection = Vector3.zero;
        cc.SimpleMove(Vector3.zero);
        transform.position = spawnPoint;
    }

    public void Enable()
    {
        foreach (Image i in damageIndicators)
        {
            i.canvasRenderer.SetAlpha(0f);
        }
        movementController.enabled = true;
        combatController.enabled = true;
        cc.enabled = true;
        worldModels.gameObject.SetActive(true);
        hudCanvas.SetActive(true);
        respawnCanvas.SetActive(false);
        this.enabled = true;
    }

    public void Disable()
    {
        movementController.enabled = false;
        combatController.enabled = false;
        cc.enabled = false;
        worldModels.gameObject.SetActive(false);
        hudCanvas.SetActive(false);
        respawnCanvas.SetActive(true);
        this.enabled = false;
    }

    private void Update()
    {
        //Hit Point Regen
        if (HPRegenTimer >= HPRegenDelay)
        {
            if (currentHP < maxHP)
            {
                currentHP = Mathf.Min(currentHP + (HPRegenRate * Time.deltaTime), maxHP);
                hudHealthBar.fillAmount = currentHP / maxHP;
            }
        }
        else
        {
            HPRegenTimer += Time.deltaTime;
        }

        if (SPRegenTimer >= SPRegenDelay)
        {
            if (currentSP < maxSP)
            {
                currentSP = Mathf.Min(currentSP + (SPRegenRate * Time.deltaTime), maxSP);
                hudShieldBar.fillAmount = currentSP / maxSP;
            }
        }
        else
        {
            SPRegenTimer += Time.deltaTime;
        }

        //Looking at Weapon Props
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, 3f))
        {
            if (hit.transform.gameObject.layer == 12)
            {
                PropWeapon prop = hit.transform.GetComponent<PropWeapon>();
                if (prop != null && controls.WeaponInteractDown && prop.WeaponID >= 0 && prop.WeaponID != combatController.CurrentWeapon.WeaponID && prop.WeaponID != combatController.SecondWeapon.WeaponID)
                {
                    prop.RemoveWeapon();

                    //Drop old weapon if theres still ammo
                    if (combatController.CurrentWeapon.TotalAmmo != 0)
                    {
                        GameObject newProp = Instantiate(combatController.CurrentWeapon.PropPrefab, transform.position + 0.5f * Vector3.up, combatController.CurrentWeapon.PropPrefab.transform.rotation);
                        newProp.GetComponent<PropWeapon>().ammo = combatController.CurrentWeapon.TotalAmmo;
                        newProp.GetComponent<Rigidbody>().AddForce(cam.forward * 10000f);
                    }

                    combatController.CurrentWeapon.gameObject.SetActive(false);
                    combatController.SwitchTo(prop.WeaponID, prop.Ammo);
                }
            }
        }
    }

    public bool OnHitWeaponProp(int weaponID, int ammo)
    {
        if (weaponID == -1)
        {
            return combatController.AddGrenade(0);
        }
        else if (weaponID == -2)
        {
            return combatController.AddGrenade(1);
        }
        else if (combatController.CurrentWeapon.WeaponID == weaponID)
        {
            bool addingAmmo = combatController.CurrentWeapon.AddAmmo(ammo);
            if (addingAmmo)
            {
                combatController.CurrentWeapon.UpdateAmmoText();
                return true;
            }
            return false;
        }
        else if (combatController.SecondWeapon.WeaponID == weaponID)
            return combatController.SecondWeapon.AddAmmo(ammo);
        else
            return false;
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
