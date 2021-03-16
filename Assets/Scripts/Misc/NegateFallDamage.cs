using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NegateFallDamage : MonoBehaviour
{
    public bool negateDamage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool NegateDamage ()
    {
        return negateDamage;
    }

    public void SetDamageNegate (bool val)
    {
        negateDamage = val;
    }
}
