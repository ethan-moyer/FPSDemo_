using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    [SerializeField] private float fadeTime = 0.1f;
    private Image image = null;
    private Color defaultColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        defaultColor = image.color;
    }

    private void OnEnable()
    {
        image.color = defaultColor;
        image.CrossFadeAlpha(0f, fadeTime, false);
    }
}
