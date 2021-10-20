using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furnace : MonoBehaviour {

	public TellParent tellParent;

	public GameObject fireLight;
	[SerializeField] private Transform barSpawn; //optional.
	public ParticleSystem smoke;
	public ParticleSystem fire;

	[SerializeField] private float fuelUse; //fuel used per item smelted
	public float fuel = 0;
	 public List<Item> currentSmeltingItem = new List<Item>();

	[HideInInspector] public float finishTime;
	public float smeltTime = 10f;
	private bool smelting;
	
	void Update() {
		if(fuel >= fuelUse  && currentSmeltingItem.Count > 0 && Time.time >= finishTime) {
			DropItem(currentSmeltingItem[currentSmeltingItem.Count - 1].smeltItem);
			StopSmelting();
		} else if(fuel < 0 && currentSmeltingItem.Count > 0) { // Not necessary unless fuel goes down on a timer
			DropItem(currentSmeltingItem[currentSmeltingItem.Count - 1]);
			StopSmelting();
		}
	}


	/*void OnTriggerStay(Collider other) {
		if(other.CompareTag("Item")) {
			ItemHandler itemHandler = other.GetComponent<ItemHandler>();
			if(itemHandler.item.type == Item.ItemType.Resource && itemHandler.item.fuel > 0) {
				Destroy(itemHandler.gameObject);
				fuel += itemHandler.item.fuel;
			} else if(itemHandler.item.type == Item.ItemType.Resource && itemHandler.item.smeltItem && fuel > 0) {
				AddItem(itemHandler.item, 1);
				Destroy(itemHandler.gameObject);
			}
		}
	}*/

	public void ReportObject (Collider col)
    {
		if (col && col.CompareTag("Item"))
		{
			ItemHandler itemHandler = col.GetComponent<ItemHandler>();
			if (itemHandler)
			{
				if (itemHandler.item.type == Item.ItemType.Resource && itemHandler.item.fuel > 0)
				{
					Destroy(itemHandler.gameObject);
					AddFuel(itemHandler.item.fuel);
				}
				else if (itemHandler.item.type == Item.ItemType.Resource && itemHandler.item.smeltItem)
				{
					AddItem(itemHandler.item, 1);
					Destroy(itemHandler.gameObject);
				}
			}
		}
	}

	public void AddItem (Item item, int amount)
    {
		if (item.smeltItem != null)
        {
			for (int i = 0; i < amount; i++) {
				currentSmeltingItem.Add(item);
			}
        } 
		if (item.fuel > 0)
        {
			fuel += amount;
        }
		StartSmelting();
	}

	public void AddFuel (float amount)
    {
		fuel += amount;
		StartSmelting();
	}

	public float FuelEfficiency ()
    {
		return fuelUse;
    }


	void StartSmelting() {
		if (fuel >= fuelUse && currentSmeltingItem.Count > 0 && !smelting)
		{
			smelting = true;
			finishTime = Time.time + smeltTime;
			smoke.Play();
			fire.Play();
			fireLight.SetActive(true);
		}
	}

	void StopSmelting() {
		fuel -= fuelUse;
		smelting = false;
		currentSmeltingItem.RemoveAt(currentSmeltingItem.Count - 1);
		smoke.Stop();
		fire.Stop();
		fireLight.SetActive(false);
		if (currentSmeltingItem.Count > 0)
        {
			StartSmelting();
        }
	}


	void DropItem(Item item) {
		GameObject smeltedItemObj = new GameObject();
		if (barSpawn)
		{
			smeltedItemObj = Instantiate(item.prefab, barSpawn.position, item.prefab.transform.rotation) as GameObject;
		}
		else
		{
			smeltedItemObj = Instantiate(item.prefab, transform.position + transform.forward * 0.25f - transform.up * 0.75f, item.prefab.transform.rotation) as GameObject;
		}
		Rigidbody objRB = smeltedItemObj.GetComponent<Rigidbody>();
		if(objRB) {
			objRB.velocity = transform.forward;
		}
	}
}