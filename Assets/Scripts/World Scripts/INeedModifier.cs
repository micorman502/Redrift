using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INeedModifier
{
    void TakeModifier(string modifier); //a script implementing this should be activated by 1 or more modifiers 
}
