using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSorter : MonoBehaviour, IItemSaveable, IGetTriggerInfo {

	[SerializeField] int saveID;
	public TellParent tellParent;

	public Transform exit;

	public Item sortingItem;

	public Renderer iconRenderer;

	public void GetTriggerInfo (Collider col)
    {
		if (col.CompareTag("Item"))
		{
			ItemHandler itemHandler = col.GetComponentInParent<ItemHandler>();
			if (itemHandler)
			{
				if (itemHandler.item == sortingItem)
				{
					itemHandler.gameObject.transform.position = exit.position;
					Rigidbody objRB = itemHandler.GetComponent<Rigidbody>();
					if (objRB)
					{
						objRB.velocity = transform.forward * 2f;
					}
				}
			}
		}
	}

	public void GetTriggerInfoRepeating (Collider col)
    {
		if (col.CompareTag("Item"))
		{
			ItemHandler itemHandler = col.GetComponentInParent<ItemHandler>();
			if (itemHandler)
			{
				if (itemHandler.item == sortingItem)
				{
					itemHandler.gameObject.transform.position = exit.position;
					Rigidbody objRB = itemHandler.GetComponent<Rigidbody>();
					if (objRB)
					{
						objRB.velocity = transform.forward * 2f;
					}
				}
			}
		}
	}

	public void SetItem(Item item) {
		sortingItem = item;
		iconRenderer.gameObject.SetActive(true);
		iconRenderer.material.mainTexture = item.icon.texture;
	}

	public void RemoveItem() {
		sortingItem = null;
		iconRenderer.material.mainTexture = null;
		iconRenderer.gameObject.SetActive(false);
	}

	public void GetData(out ItemSaveData data, out ObjectSaveData objData, out bool dontSave)
	{
		ItemSaveData newData = new ItemSaveData();
		ObjectSaveData newObjData = new ObjectSaveData(transform.position, transform.rotation, saveID);

		if (sortingItem)
		{
			newData.itemID = sortingItem.id;
		}
		else
		{
			newData.itemID = -1;
		}

		data = newData;
		objData = newObjData;
		dontSave = false;
	}

	public void SetData(ItemSaveData data, ObjectSaveData objData)
	{
		transform.position = objData.position;
		transform.rotation = objData.rotation;

		if (data.itemID != -1)
		{
			SetItem(SaveManager.Instance.FindItem(data.itemID));
		}
	}
}
