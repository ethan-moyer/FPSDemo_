using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [HideInInspector] public bool zoomed = false;
    [SerializeField] private float xSensitivity = 100.0f;
    [SerializeField] private float ySensitivity = 100.0f;
    [SerializeField] private float zoomedPercent = .5f;
    [SerializeField] private Transform cam = null;

    private PlayerInputReader controls;
    private float xAxis;
    private float yAxis;
    private float xRotation;

    private void Awake()
    {
        controls = GetComponent<PlayerInputReader>();
        xRotation = cam.localRotation.x;
    }

    private void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            xAxis = controls.LookDir.x * xSensitivity * Time.deltaTime;
            yAxis = controls.LookDir.y * ySensitivity * Time.deltaTime;
            if (zoomed)
            {
                xAxis *= zoomedPercent;
                yAxis *= zoomedPercent;
            }
        }
        
        xRotation -= yAxis;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * xAxis);
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;

    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}
