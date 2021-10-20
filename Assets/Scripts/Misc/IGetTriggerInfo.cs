using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGetTriggerInfo
{
    void GetTriggerInfo(Collider col);
    void GetTriggerInfoRepeating(Collider col);
}
