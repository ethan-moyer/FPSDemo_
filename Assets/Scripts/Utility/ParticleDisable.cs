using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDisable : MonoBehaviour
{
    [SerializeField] private float lifetime = 2f;

    private void OnEnable()
    {
        StartCoroutine(WaitAndDisable());
    }

    private IEnumerator WaitAndDisable()
    {
        yield return new WaitForSeconds(lifetime);
        gameObject.SetActive(false);
    }
}
