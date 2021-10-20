using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingSlot : MonoBehaviour {
	WorldItem item;
	[SerializeField] Image icon;
	[SerializeField] Text amountText;

	Inventory inventory;

	void Start() {
		inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
	}

	public void Setup (WorldItem newItem)
	{
		item = newItem;
		icon.sprite = item.item.icon;
		amountText.text = item.amount.ToString();
	}

	public void OnItemPointerEnter() {
		if(item.item) {
			InventoryEvents.SetHoveredItem(item.item, null);
		}
	}

	public void OnItemPointerExit() {
		InventoryEvents.LeaveHoveredItem();
	}
}
