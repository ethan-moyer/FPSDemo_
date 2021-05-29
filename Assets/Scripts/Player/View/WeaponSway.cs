using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("Weapon Bobbing")]
    [SerializeField] private Transform viewModelPivot = null;
    [SerializeField] private float verticalBobAmount = 0.5f;
    [SerializeField] private float horizontalBobAmount = 0.5f;
    [SerializeField] private float bobSpeed = 1f;
    [Header("Weapon Sway")]
    [SerializeField] private float verticalSwayAmount = 5f;
    [SerializeField] private float horizontalSwayAmount = 5f;
    [SerializeField] private float swaySpeed = 3f;
    private PlayerMovementController movementController = null;
    private PlayerInputReader controls = null;

    private void Awake()
    {
        movementController = GetComponent<PlayerMovementController>();
        controls = GetComponent<PlayerInputReader>();
    }

    private void Update()
    {
        if (movementController.CurrentStateIs(typeof(WalkState)))
        {
            float x = horizontalBobAmount * Mathf.Sin(bobSpeed * 2 * Time.time);
            float y = verticalBobAmount * Mathf.Sin(bobSpeed * Time.time);
            viewModelPivot.localPosition += new Vector3(y, x, 0f) * controls.WalkDir.magnitude;

            Quaternion newRotation = Quaternion.Euler(Mathf.Clamp(controls.LookDir.y, -verticalSwayAmount, verticalSwayAmount), Mathf.Clamp(controls.LookDir.x, -horizontalSwayAmount, horizontalSwayAmount), 0f);
            viewModelPivot.localRotation = Quaternion.Lerp(viewModelPivot.localRotation, newRotation, swaySpeed * Time.deltaTime);
        }
    }
}
