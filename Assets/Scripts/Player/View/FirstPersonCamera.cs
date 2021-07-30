using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;

public class FirstPersonCamera : MonoBehaviour
{
    [HideInInspector] public bool zoomed = false;
    [SerializeField] private Vector2 mouseSensitivity = Vector2.one * 100f;
    [SerializeField] private Vector2 gamepadSensitivity = Vector2.one * 100f;
    [SerializeField] private float gamepadAccelerationStart = 0.2f;
    [SerializeField] private float gamepadAcceleration = 1f;
    [SerializeField] private float maxTurnRate = 1f;
    [SerializeField] private float zoomedPercent = .5f;
    [SerializeField] private Transform cam = null;
    [SerializeField] private LayerMask aimAssistMask;
    [SerializeField] private Image reticle;

    private PlayerInputReader controls;
    private PlayerInput playerInput;
    private PlayerCombatController combatController;
    private float xAxis;
    private float yAxis;
    private float xRotation;
    private float gamepadAccelerationStartTimer;

    private void Awake()
    {
        controls = GetComponent<PlayerInputReader>();
        playerInput = GetComponent<PlayerInput>();
        combatController = GetComponent<PlayerCombatController>();
        xRotation = cam.localRotation.x;
        gamepadAccelerationStartTimer = 0f;
    }

    private void Update()
    {
        //Move via input
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Vector2 lookDir = controls.LookDir;
            if (zoomed)
            {
                lookDir *= zoomedPercent;
            }

            if (playerInput.currentControlScheme == "Keyboard")
            {
                xAxis = lookDir.x * mouseSensitivity.x * Time.deltaTime;
                yAxis = lookDir.y * mouseSensitivity.y * Time.deltaTime;
            }
            else
            {
                if (controls.LookDir.magnitude > 0f)
                {
                    yAxis = Mathf.Lerp(yAxis, lookDir.y * gamepadSensitivity.y * Time.deltaTime, gamepadAcceleration * Time.deltaTime);
                    xAxis = Mathf.Lerp(xAxis, lookDir.x * gamepadSensitivity.x * Time.deltaTime, gamepadAcceleration * Time.deltaTime);
                }
                else
                {
                    yAxis = 0f;
                    xAxis = 0f;
                }
            }
        }

        //Reticle Change & Aim Assist
        Collider[] colliders = Physics.OverlapSphere(cam.position, combatController.CurrentWeapon.maxDistance, aimAssistMask);
        reticle.color = Color.white;
        this.gameObject.layer = 2;
        foreach (Collider col in colliders)
        {
            RaycastHit hit;
            if (ConeCast(col, out hit))
            {
                reticle.color = Color.red;

                //Aim Assist
                if (playerInput.currentControlScheme == "Gamepad" && ((xAxis != 0f && yAxis != 0f) || controls.WalkDir.magnitude > 0f))
                {
                    Vector3 towardsPoint = (hit.point - cam.position).normalized;
                    Vector3 difference = towardsPoint - cam.forward;
                    Vector3 localDifference = cam.transform.InverseTransformDirection(difference);
                    xAxis += localDifference.x * 2f;
                    yAxis += localDifference.y * 2f;
                }
            }
        }
        this.gameObject.layer = 9;

        //Apply Rotation
        xRotation -= yAxis;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * xAxis);
    }

    private bool ConeCast(Collider col, out RaycastHit hit)
    {
        //Check if closest point to camera is in cone
        Vector3 target = col.ClosestPoint(cam.position + cam.forward * Vector3.Distance(cam.position, col.transform.position));
        if (Physics.Raycast(cam.position, target - cam.position, out hit, combatController.CurrentWeapon.maxDistance))
        {
            if (hit.transform == col.transform)
            {
                //point = hit.point;
                float distance = Vector3.Dot(hit.point - cam.position, cam.forward);
                float radius = (distance / combatController.CurrentWeapon.maxDistance) * combatController.CurrentWeapon.coneRadius;
                float orthDistance = Vector3.Magnitude((hit.point - cam.position) - (cam.forward * distance));
                bool inCone = orthDistance < radius;
                return inCone;
            }
        }
        return false;
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
