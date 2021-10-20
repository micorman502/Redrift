using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WorldItem
{
    public Item item;
    public int amount;

    public void Clear ()
    {
        item = null;
        amount = 0;
    }

    public bool AtMaxStack ()
    {
        if (amount >= item.maxStackCount)
        {
            return true;
        } else
        {
            return false;
        }
    }

    public WorldItem(Item _item, int _amount)
    {
        this.item = _item;
        this.amount = _amount;
    }
}
