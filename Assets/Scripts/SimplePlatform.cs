using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlatform : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float maxDistance;
    private float period;
    private Vector3 previousPos;

    public Vector3 velocity;

    void Start()
    {
        period = Mathf.PI / maxDistance;
        previousPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * speed * Mathf.Sin(period * Time.time) * Time.deltaTime);
        if (previousPos == transform.position)
        {
            velocity = Vector3.zero;
        }
        else
        {
            velocity = (transform.position - previousPos);
            previousPos = transform.position;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        print(hit.gameObject.name);
    }
}
