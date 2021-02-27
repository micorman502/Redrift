using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    [SerializeField] private Item heldItem;
    [SerializeField] private int stackSize;
    [SerializeField] private SpriteRenderer itemDisplay;
    [SerializeField] private Sprite nullItem;
    [SerializeField] private TextMesh text;
    [SerializeField] private string nullText;

    void Start()
    {
        if (stackSize > 0)
        {
            itemDisplay.sprite = heldItem.icon;
            text.text = stackSize.ToString();
        }
        else
        {
            itemDisplay.sprite = nullItem;
            text.text = nullText;
        }
    }

    public void LoadValues (int itemAmt, Item item) //Used for saving only!
    {
        stackSize = itemAmt;
        heldItem = item;
    }

    public void LoadNullValues() //Used for saving only!
    {
        stackSize = 0;
        heldItem = null;
    }
    public void AddItem (Item thisItem, bool rejected)
    {
        bool r = true;
        if (thisItem == heldItem)
        {
            if (stackSize + 1 <= heldItem.maxStackCount * 2)
            {
                stackSize += 1;
                r = false;
                itemDisplay.sprite = heldItem.icon;
                text.text = stackSize.ToString();
            } else
            {
                r = true;
            }
        } else if (heldItem == null)
        {
            heldItem = thisItem;
            stackSize = 1;
            itemDisplay.sprite = heldItem.icon;
            text.text = stackSize.ToString();
        } else
        {
            r = true;
        }
        rejected = r;

    }

    public void RemoveItem (int amountRemoved, out int itemsOut)
    {
        if (stackSize - amountRemoved > 0) {
            itemsOut = stackSize - amountRemoved;
            stackSize -= amountRemoved;
            itemDisplay.sprite = heldItem.icon;
            text.text = stackSize.ToString();
        } else
        {
            itemsOut = stackSize;
            heldItem = null;
            stackSize = 0;
            itemDisplay.sprite = nullItem;
            text.text = nullText;
        }
    }

    public void RemoveItem (int amountRemoved) //if items being taken is over 1
    {
        if (stackSize - amountRemoved > 0)
        {
            stackSize -= amountRemoved;
            itemDisplay.sprite = heldItem.icon;
            text.text = stackSize.ToString();
        }
        else
        {
            heldItem = null;
            stackSize = 0;
            itemDisplay.sprite = nullItem;
            text.text = nullText;
        }
    }

    public Item GetItem (out bool noItem)
    {
        if (heldItem)
        {
            noItem = false;
        } else
        {
            noItem = true;
        }
        return heldItem;
    }

    public Item GetItem()
    {
        return heldItem;
    }

    public int ItemStack ()
    {
        return stackSize;
    }
}
