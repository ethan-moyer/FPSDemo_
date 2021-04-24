using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [SerializeField] private PlayerInputReader controls;
    [SerializeField] private float swayAmount;
    [SerializeField] private float maxRotX;
    [SerializeField] private float maxRotY;
    [SerializeField] private float swayAcceleration;
    private Vector3 startRot;
    private Vector3 newRot;

    void Start()
    {
        startRot = transform.localRotation.eulerAngles;
        newRot = transform.localRotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Approximately(controls.LookX, 0f))
            newRot.y = Mathf.Lerp(newRot.y, startRot.y, swayAcceleration * Time.deltaTime);
        else
            newRot.y += controls.LookX * swayAmount * Time.deltaTime;

        if (Mathf.Approximately(controls.LookY, 0f))
            newRot.x = Mathf.Lerp(newRot.x, startRot.x, swayAcceleration * Time.deltaTime);
        else
            newRot.x -= controls.LookY * swayAmount * Time.deltaTime;

        newRot.x = Mathf.Clamp(newRot.x, -maxRotX, maxRotX);
        newRot.y = Mathf.Clamp(newRot.y, startRot.y - maxRotY, startRot.y + maxRotY);

        transform.localRotation = Quaternion.Euler(newRot);
    }
}
