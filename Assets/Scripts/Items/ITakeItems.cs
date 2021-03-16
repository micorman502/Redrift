using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITakeItems
{
    void AddItem(Item item, int amount, out bool rejected);
    void AddItem(Item item, out bool rejected);
}
