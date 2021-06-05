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
                changeFOV.Invoke(zoomFOVMultiplier);
            }
            else if (zoomLevel == 1)
            {
                zoomLevel = 2;
                isZoomed = true;
                changeFOV.Invoke(secondZoomFOVMultiplier);
            }
            else
            {
                zoomLevel = 0;
                isZoomed = false;
                changeFOV.Invoke(-1);
            }
        }
        else
        {
            isZoomed = false;
            changeFOV.Invoke(-1);
        }
    }

    protected override void Fire()
    {
        currentState = States.Firing;
        timeTillNextFire = 1 / fireRate;
        currentMagAmmo -= 1;
        effect.Play();
        triggerAnimation.Invoke("Fire");
        combatController.UpdateAmmoText(AmmoToText());
        GameObject sniperTrail = ObjectPooler.SharedInstance.GetPooledObject(3);

        RaycastHit hit;
        combatController.gameObject.layer = 2;
        Vector3 randDirection = (cam.forward.normalized * maxDistance) + (cam.right.normalized * Random.Range(-1f, 1f) * coneRadius) + (cam.up.normalized * Random.Range(-1f, 1f) * coneRadius);
        Vector3 endPoint = cam.position + cam.forward * maxDistance;

        if (Physics.Raycast(cam.position, randDirection, out hit, maxDistance))
        {
            endPoint = hit.point;
            if (hit.transform.gameObject.layer == 12)
            {
                //Hit a Pickup or Prop.
                PlaceEffect(1, hit.point, hit.normal);
                Prop prop = hit.transform.GetComponent<Prop>();
                if (prop != null)
                {
                    prop.Hit(hit.point, cam.forward * 10f);
                }
            }
            else if (hit.transform.gameObject.layer == 10)
            {
                //Hit Terrain.
                PlaceEffect(0, hit.point, hit.normal);
            }
        }
        sniperTrail.GetComponent<SniperTrail>().Positions = new Vector3[] { cam.TransformPoint(viewModel.offset), endPoint };
        sniperTrail.SetActive(true);
        
        combatController.gameObject.layer = 9;
    }
}
