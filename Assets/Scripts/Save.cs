using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Save {
	public ObjectSaveData playerTransform;
	public float playerHealth;
	public float playerHunger;

	public List<ObjectSaveData> savedObjects = new List<ObjectSaveData>();
	public List<ItemSaveData> savedObjectsInfo = new List<ItemSaveData>();

	public List<SerializedWorldItem> inventoryItems = new List<SerializedWorldItem>();

	public List<int> achievementIDs = new List<int>();

	public DateTime saveTime;

	public int worldType;

	public int difficulty;

	public int mode;

	public bool playerDead;

	public SerializableVector3 playerVelocity;
}

[Serializable]
public class ObjectSaveData
{
	public SerializableVector3 position;
	public SerializableQuaternion rotation;
	public int objectID;

	public ObjectSaveData(Vector3 _position, Quaternion _rotation, int _ID)
    {
		position = _position;
		rotation = _rotation;
		objectID = _ID;
    }

	public ObjectSaveData()
	{
		position = Vector3.zero;
		rotation = Quaternion.identity;
		objectID = -1;
	}
}

[Serializable]
public class SerializedWorldItem {
	public int id;
	public int amount;

	public SerializedWorldItem(int _id, int _amt)
    {
		id = _id;
		amount = _amt;
    }

	public static implicit operator SerializedWorldItem(WorldItem item)
	{
		return new SerializedWorldItem(item.item.id, item.amount);
	}
}

[Serializable]
public class ItemSaveData {
	public float floatVal;
	public int num;
	public bool boolVal;
	public int itemID;
	public List<int> itemIDs;
	public List<int> itemAmounts;
}