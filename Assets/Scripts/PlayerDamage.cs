using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    [SerializeField] float damage;
    public float GetDamage()
    {
        return damage;
    }
}
