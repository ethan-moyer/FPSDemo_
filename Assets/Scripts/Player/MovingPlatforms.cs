using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatforms : MonoBehaviour
{
    private CharacterController cc;
    private Transform currentPlatform;
    private Vector3 globalPoint;
    private Vector3 localPoint;
    private Vector3 moveDirection;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPlatform != null)
        {
            Vector3 newGlobalPoint = currentPlatform.TransformPoint(localPoint);
            moveDirection = newGlobalPoint - globalPoint;
            if (moveDirection.magnitude > 0.01f)
            {
                cc.Move(moveDirection);
            }
            UpdatePlatform();
        }
        else
        {
            if (moveDirection.magnitude > 0.01f)
            {
                moveDirection = Vector3.Lerp(moveDirection, Vector3.zero, Time.deltaTime);
                cc.Move(moveDirection);
            }
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.moveDirection.y < -0.9f && hit.normal.y > 0.41)
        {
            if (currentPlatform != hit.collider.transform && hit.gameObject.layer == 11)
            {
                currentPlatform = hit.collider.transform;
                UpdatePlatform();
            }
        }
        else
        {
            currentPlatform = null;
        }
    }

    private void UpdatePlatform()
    {
        globalPoint = transform.position;
        localPoint = currentPlatform.InverseTransformPoint(transform.position);
    }
}
