using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperTrail : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.5833333f;
    private float timer = 0f;
    private float rateOfChange = 0f;
    private LineRenderer line = null;

    public Vector3[] Positions { get; set; }

    private void Awake()
    {
        rateOfChange = 1f / lifetime;
        line = GetComponent<LineRenderer>();
        Positions = new Vector3[2];
    }

    private void OnEnable()
    {
        line.SetPositions(Positions);
        timer = 0f;
    }

    private void Update()
    {
        if (timer >= lifetime)
        {
            gameObject.SetActive(false);
        }
        else
        {
            float opacity = 1 - (rateOfChange * timer);
            line.startColor = new Color(1, 1, 1, opacity);
            line.endColor = new Color(1, 1, 1, opacity);
            timer += Time.deltaTime;
        }
    }
}
