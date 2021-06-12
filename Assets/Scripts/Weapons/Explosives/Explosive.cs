using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    [SerializeField] private float HPDamage = 0f;
    [SerializeField] private float SPDamage = 0f;
    [SerializeField] private int explosionEffect = 2;
    [SerializeField] private float sphereRadius;
    [SerializeField] private LayerMask targets;
    [SerializeField] private AudioClip sound = null;
    private VirtualAudioSource virtualAudioSource;

    private void Awake()
    {
        virtualAudioSource = GetComponent<VirtualAudioSource>();
    }

    public void Explode()
    {
        GameObject effect = ObjectPooler.SharedInstance.GetPooledObject(explosionEffect);
        if (effect != null)
        {
            effect.transform.position = transform.position;
            effect.SetActive(true);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, sphereRadius, targets);
        foreach(Collider collider in colliders)
        {
            Vector3 point = collider.ClosestPoint(transform.position);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, point - transform.position, out hit, sphereRadius))
            {
                float falloff = 1 - (Vector3.Distance(transform.position, hit.point) / sphereRadius);
                if (hit.transform == collider.transform)
                {
                    if (hit.transform.gameObject.layer == 9)
                    {
                        PlayerController player = hit.transform.GetComponent<PlayerController>();
                        if (player != null)
                        {
                            player.DamageHit(HPDamage * falloff, SPDamage * falloff);
                            player.PhysicsHit(point - transform.position, falloff * (HPDamage) * 0.15f);
                        }
                    }
                    else if (hit.transform.gameObject.layer == 12)
                    {
                        Prop prop = hit.transform.GetComponent<Prop>();
                        if (prop != null)
                        {
                            prop.Hit(point, hit.normal * falloff * -70f);
                        }
                    }
                }
            }
        }
        virtualAudioSource.Play(sound);
        Destroy(this.gameObject);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
    }
}
