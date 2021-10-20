using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furnace : MonoBehaviour, IItemSaveable, IGetTriggerInfo {

	[SerializeField] int saveID;

	public GameObject fireLight;
	public ParticleSystem smoke;
	public ParticleSystem fire;

	[HideInInspector] public float fuel = 0;
	[HideInInspector] public Item currentSmeltingItem;

	public float smeltTime = 10f;

	public void GetTriggerInfo (Collider col)
    {
		if (col.CompareTag("Item"))
		{
			ItemHandler itemHandler = col.GetComponent<ItemHandler>();
			if (!itemHandler)
				return;
			if (itemHandler.item.type == Item.ItemType.Resource && itemHandler.item.fuel > 0)
			{
				Destroy(itemHandler.gameObject);
				fuel += itemHandler.item.fuel;
			}
			else if (itemHandler.item.type == Item.ItemType.Resource && itemHandler.item.smeltItem && !currentSmeltingItem && fuel > 0)
			{
				currentSmeltingItem = itemHandler.item;
				Destroy(itemHandler.gameObject);
				Check();
			}
		}
	}

	public void GetTriggerInfoRepeating (Collider col)
    {
		GetTriggerInfo(col);
    }

	void Check ()
    {
		if (currentSmeltingItem && fuel >= 1)
        {
			StartSmelting(currentSmeltingItem);
        }
    }

	void StartSmelting(Item item) {
		//finishTime = Time.time + smeltTime;
		currentSmeltingItem = item;
		smoke.Play();
		fire.Play();
		fireLight.SetActive(true);
		Invoke("FinishSmelting", smeltTime);
	}

	void FinishSmelting ()
    {
		DropItem(currentSmeltingItem.smeltItem);
		fuel--;
		StopSmelting();
	}

	void StopSmelting() {
		currentSmeltingItem = null;
		smoke.Stop();
		fire.Stop();
		fireLight.SetActive(false);
	}

	void DropItem(Item item) {
		GameObject smeltedItemObj = Instantiate(item.prefab, transform.position + transform.forward * 0.25f - transform.up * 0.75f, item.prefab.transform.rotation) as GameObject;
		Rigidbody objRB = smeltedItemObj.GetComponent<Rigidbody>();
		if(objRB) {
			objRB.velocity = transform.forward;
		}
	}

	public void GetData (out ItemSaveData data,out ObjectSaveData objData, out bool dontSave)
    {
		ItemSaveData newData = new ItemSaveData();
		ObjectSaveData newObjData = new ObjectSaveData(transform.position, transform.rotation, saveID);

		newData.floatVal = fuel;
		if (currentSmeltingItem)
		{
			newData.itemID = currentSmeltingItem.id;
		}
		else
		{
			newData.itemID = -1;
		}

		data = newData;
		objData = newObjData;
		dontSave = false;
    }

	public void SetData (ItemSaveData data, ObjectSaveData objData)
    {
		transform.position = objData.position;
		transform.rotation = objData.rotation;
		fuel = data.floatVal;
		if (data.itemID != -1)
		{
			currentSmeltingItem = SaveManager.Instance.FindItem(data.itemID);
		}
		Check();
	}
}