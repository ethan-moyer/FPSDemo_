using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Explosive))]
public class Rocket : Projectile
{
    private Explosive explosive;

    protected override void Awake()
    {
        base.Awake();
        explosive = GetComponent<Explosive>();
    }

    protected override void Hit()
    {
        explosive.Explode();
    }
}
