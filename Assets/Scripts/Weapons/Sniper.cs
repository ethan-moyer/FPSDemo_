using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : Gun
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
}
