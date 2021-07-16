using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class ModularCooldownWeapon : ModularWeapon
{
    public FloatEvent UpdatingCooldownMeter;
    [SerializeField] private float heatLimit = 1f;
    [SerializeField] private float heatPerAttack = 0.1f;
    [SerializeField] private float coolingRate = 1f;
    private float heat = 0f;

    private void OnEnable()
    {
        heat = 0f;
        UpdatingCooldownMeter.Invoke(heat / heatLimit);
    }

    public override void SetAmmo(int ammo)
    {
        if (ammo == -1)
            currentAmmo = maxAmmo;
        else if (ammo <= maxAmmo)
            currentAmmo = ammo;
    }

    public override void UpdateAmmoText()
    {
        UpdatingAmmoText.Invoke($"{currentAmmo}");
    }

    public override void PrimaryAction()
    {
        if (CurrentState == States.Idle && currentAmmo > 0)
        {
            if (stayZoomed == false)
                SetZoomLevel(0);

            attack.Attack(player, this, cam);
            attackTimer = 1 / attackRate;
            CurrentState = States.Firing;
            currentAmmo -= 1;
            UpdateAmmoText();

            heat += heatPerAttack;
            if (heat >= heatLimit)
                StartCoroutine(Overheat());
        }
    }

    public override void SecondaryAction()
    {
        if (CurrentState == States.Idle)
        {
            SetZoomLevel(currentZoomLevel + 1);
        }
    }

    public override void TertiaryAction()
    {
        
    }

    private IEnumerator Overheat()
    {
        print("Overheat!");
        TriggeringAnimation.Invoke("Reload");
        CurrentState = States.Busy;
        yield return new WaitUntil(() => heat <= 0);
        CurrentState = States.Idle;
    }

    protected override void Update()
    {
        base.Update();
        if (heat > 0)
        {
            heat = Mathf.Max(heat - (Time.deltaTime * coolingRate), 0f);
            UpdatingCooldownMeter.Invoke(heat / heatLimit);
        }
    }
}
