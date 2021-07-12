using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShieldRenderer : MonoBehaviour
{
    [SerializeField] private float fadeTime = 1f;
    [SerializeField] private float minimumAlpha = 3f;
    [SerializeField] private float maximumAlpha = 20f;
    private float timer = 0f;
    private MeshRenderer meshRenderer;

    public float Alpha { get; set; }

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        timer = 0f;
    }

    private void Update()
    {
        if (timer < fadeTime)
        {
            meshRenderer.material.SetFloat("Vector1_7BD56E3E", Mathf.Lerp(minimumAlpha, maximumAlpha, timer / fadeTime));
            timer += Time.deltaTime;
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }
}
