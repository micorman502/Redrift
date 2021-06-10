using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NegateFallDamage : MonoBehaviour
{
    [SerializeField] bool negateDamage;
    [SerializeField] float fallDamageMult;

    public bool NegateDamage ()
    {
        return negateDamage;
    }

    public float GetFallDamageMult ()
    {
        return fallDamageMult;
    }

    public void SetDamageNegate (bool val)
    {
        negateDamage = val;
    }
}
