using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITakeFallDamage
{
    void TakeFallDamage(float amount);
    void TakeFallDamage(GameObject source);
}
