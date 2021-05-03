using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Prop : MonoBehaviour
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Hit(Vector3 point, Vector3 force)
    {
        rb.AddForceAtPosition(force, point, ForceMode.Impulse);
    }
}
