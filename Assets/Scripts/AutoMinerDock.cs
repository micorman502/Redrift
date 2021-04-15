using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMinerDock : MonoBehaviour, ITakeInput
{
    [SerializeField] List<Item> items = new List<Item>();
    [SerializeField] List<int> itemAmounts = new List<int>();
	[SerializeField] int maxItems;
	[SerializeField] bool automated;
	[SerializeField] Transform dropPoint;
	[SerializeField] Light automationIndicator;
	[SerializeField] VisualBar itemBar;
	int totalItems;
    // Start is called before the first frame update
    void Start()
    {
		UpdateTotalItems();
		automationIndicator.enabled = automated;
		InvokeRepeating("AutoUnload", 0.35f, 0.35f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
			UpdateTotalItems();
        }
    }

	public void GetValues (out List<Item> passedItems, out List<int> amounts, out bool automationState)
    {
		passedItems = items;
		amounts = itemAmounts;
		automationState = automated;
    }

	public void LoadValues (List<Item> passedItems, List<int> amounts, bool automationState)
    {
		items = passedItems;
		itemAmounts = amounts;
		automated = automationState;
		automationIndicator.enabled = automated;
		UpdateTotalItems();
	}

	public void TakeInput (string input)
    {
		if (input == "unload")
        {
			Unload();
        }
		if (input == "automate")
        {
			automated = !automated;
			automationIndicator.enabled = automated;
		}
    }

	public void AutoUnload ()
    {
		if (automated)
        {
			Unload();
        }
    }

    public void UnloadMiner (AutoMiner miner, out bool accepted)
    {
		accepted = false;
		UpdateTotalItems();
		Debug.Log(miner.GetItems().Count + " UnloadMiner");
		for (int i = 0; i < miner.GetItems().Count; i++)
		{
			if (totalItems + miner.GetStackSize(i) <= maxItems && maxItems > 0)
			{
				miner.TakeItemStack(i, out int taken, out Item item);
				AddItem(item, taken);
				accepted = true;
			}
			UpdateTotalItems();
		}
		miner.CheckItems();
    }

	public void Unload ()
    {
		Unload(0);
    }

	public void Unload (int itemIndex)
    {
		Item item = ScriptableObject.CreateInstance<Item>();
		if (items.Count > itemIndex)
		{
			item = items[itemIndex];
		}
		RemoveItem(itemIndex, 1, out int itemsReturned);
		if (itemsReturned > 0)
        {
			Instantiate(item.prefab, dropPoint.position, Quaternion.identity);
        }
    }

    void AddItem(Item item, int amount)
    {
		bool hasItem = false;
        int foundID = 0;

        int i = 0;
		foreach (Item _item in items)
		{
			if (_item != null)
			{
				if (_item.id == item.id)
				{
					hasItem = true;
					itemAmounts[i] += amount;
					foundID = i;
				}
				i++;
			}
		}

        if (!hasItem)
        {
            items.Add(item);
            itemAmounts.Add(amount);
			foundID = items.Count - 1;
        }
        if (automated)
        {
			for (int o = 0; o < amount; o++) {
				Unload(o);
			}
        }

    }

	void RemoveItem (Item item, int amount)
    {
		RemoveItem(item, amount, out int disposed);
	}

	void RemoveItem (Item item, int amount, out int returned)
    {
		int tempReturn = 0;

		int i = 0;
		foreach (Item _item in items)
		{
			if (_item.id == item.id)
			{
				itemAmounts[i] -= amount;
				tempReturn = itemAmounts[i];
			}
			if (itemAmounts[i] <= 0)
			{
				tempReturn = 0;
				itemAmounts.RemoveAt(i);
				items.RemoveAt(i);
			}
			i++;
		}
		returned = tempReturn;
		UpdateTotalItems();
	}

	void RemoveItem(int itemIndex, int amount, out int returned)
	{
		int deletedItem = -1;
		int tempReturn = 0;
		int i = 0;
		foreach (Item _item in items)
		{
			if (i == itemIndex)
			{
				itemAmounts[i] -= amount;
				tempReturn = itemAmounts[i];
			}
			if (itemAmounts[i] <= 0)
			{
				deletedItem = i;
			}
			i++;
		}

		if (deletedItem > -1)
        {
			tempReturn = 0;
			itemAmounts.RemoveAt(deletedItem);
			items.RemoveAt(deletedItem);
		}
		returned = tempReturn;
		UpdateTotalItems();
	}

	void UpdateTotalItems ()
    {
		totalItems = 0;
		foreach (int current in itemAmounts)
        {
			totalItems += current;
        }
		float divided = totalItems / (float)maxItems;
		itemBar.SetPercentNormalized(divided);
	}
}
