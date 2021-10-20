using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceHandler : MonoBehaviour, IItemSaveable {
	[SerializeField] bool dontSave;
	[SerializeField] int saveID;
	public Resource resource;

	public int health;

	void Awake() {
		HiveMind.Instance.AddResource(this);
	}

	void Start ()
    {
		if (health == 0)
		{
			health = resource.maxGathers;
		}
	}

	public void Gather(int amount) {
		health -= amount;
		if(health <= 0 && !resource.infiniteGathers) {
			HiveMind.Instance.RemoveResource(this);
			if(resource.id == 5) { // This resource is a tree
				GetComponent<TreeResource>().DropFruits();
			}
			Destroy(gameObject);
		}
	}

	public void GetData(out ItemSaveData data, out ObjectSaveData objData, out bool _dontSave)
	{
		ItemSaveData newData = new ItemSaveData();
		ObjectSaveData newObjData = new ObjectSaveData(transform.position, transform.rotation, saveID);

		newData.num = health;

		data = newData;
		objData = newObjData;
		_dontSave = dontSave;
	}

	public void SetData(ItemSaveData data, ObjectSaveData objData)
	{
		transform.position = objData.position;
		transform.rotation = objData.rotation;

		health = data.num;
	}
}
