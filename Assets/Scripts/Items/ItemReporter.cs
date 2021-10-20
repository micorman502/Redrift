using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemReporter : MonoBehaviour //basically FurancePhysicalManager but i havent setup the interfaces yet so reee
{
    public Component holder;
    private bool holderTakeItems;

    void Start()
    {
        if (holder.GetComponent<ITakeItems>() != null)
        {
            holderTakeItems = true;
        } else
        {
            holderTakeItems = false;
        }
    }
    void OnTriggerEnter(Collider other) 
    {
        if (holderTakeItems)
        {
            ItemHandler handler = other.GetComponent<ItemHandler>();
            if (handler)
            {
                holder.GetComponent<ITakeItems>().AddItem(handler.item, 1, out bool rejected);
                if (!rejected)
                {
                    Destroy(handler.gameObject);
                }
            }
        }

    }
}
