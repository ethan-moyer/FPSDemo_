using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : ReloadableWeapon
{
    [Header("Sniper")]
    [SerializeField] private float secondZoomFOVMultiplier = 0.3f;
    private int zoomLevel = 0;

    protected override void Zoom(bool shouldZoom)
    {
        if (shouldZoom == true)
        {
            if (zoomLevel == 0)
            {
                zoomLevel = 1;
                isZoomed = true;
                ChangeFOV.Invoke(zoomFOVMultiplier);
            }
            else if (zoomLevel == 1)
            {
                zoomLevel = 2;
                isZoomed = true;
                ChangeFOV.Invoke(secondZoomFOVMultiplier);
            }
            else
            {
                zoomLevel = 0;
                isZoomed = false;
                ChangeFOV.Invoke(-1);
            }
        }
        else
        {
            isZoomed = false;
            ChangeFOV.Invoke(-1);
        }
    }

    protected override void Fire()
    {
        currentState = States.Firing;
        timeTillNextFire = 1 / fireRate;
        currentMagAmmo -= 1;
        effect.Play();
        PlayAudioClip.Invoke(fireClip);
        TriggerAnimation.Invoke("Fire");
        combatController.UpdateAmmoText(AmmoToText());
        GameObject sniperTrail = ObjectPooler.SharedInstance.GetPooledObject(3);

        combatController.gameObject.layer = 2;
        Vector3 randDirection = (cam.forward.normalized * maxDistance) + (cam.right.normalized * Random.Range(-1f, 1f) * coneRadius) + (cam.up.normalized * Random.Range(-1f, 1f) * coneRadius);
        Vector3 endPoint = ShootRay(randDirection);
        sniperTrail.GetComponent<SniperTrail>().Positions = new Vector3[] { cam.TransformPoint(viewModel.offset), endPoint };
        sniperTrail.SetActive(true);
        
        combatController.gameObject.layer = 9;
    }
}
