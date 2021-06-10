using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Save {
	public SerializableVector3 playerPosition;
	public SerializableQuaternion playerRotation;
	public float playerHealth;
	public float playerHunger;
	public int playerLastEatenItem;

	public List<SerializableVector3> worldItemPositions = new List<SerializableVector3>();
	public List<SerializableQuaternion> worldItemRotations = new List<SerializableQuaternion>();
	public List<int> worldItemIDs = new List<int>();
	public List<bool> worldItemHasRigidbodies = new List<bool>();

	public List<SerializableVector3> worldResourcePositions = new List<SerializableVector3>();
	public List<SerializableQuaternion> worldResourceRotations = new List<SerializableQuaternion>();
	public List<float> worldResourceGrowthStates = new List<float>();
	public List<int> worldResourceIDs = new List<int>();
	public List<int> worldResourceHealths = new List<int>();

	public List<int> inventoryItemIDs = new List<int>();
	public List<int> inventoryItemAmounts = new List<int>();

	public List<int> achievementIDs = new List<int>();

	public List<MeteorSaveData> meteorSaveData = new List<MeteorSaveData>();

	public List<SerializedApple> apples = new List<SerializedApple>();

	public List<ItemSaveData> itemSaveData = new List<ItemSaveData>();

	public List<SerializableVector3> smallIslandPositions = new List<SerializableVector3>();
	public List<SerializableVector3> smallDarkIslandPositions = new List<SerializableVector3>();

	//public List<int> treeAssociation = new List<int>();

	public DateTime saveTime;

	public string version = "1.7.2"; //not necesary to change

	public int worldType;

	public int difficulty;

	public int mode;

	public string modifier;

	public bool playerDead;

	public SerializableVector3 playerVelocity;
}

[Serializable]
public class ItemSaveData {

	public float fuel;
	public int itemID;
	public int itemAmount;
	public List<int> itemIDs = new List<int>();
	public List<int> itemAmounts;
	public int num;
	public bool bit;
}

[Serializable]
public class MeteorSaveData
{
	public SerializableVector3 path;
	public SerializableVector3 pos;
	public float size;
	public float speed;
	public int penetrationTicks;
}

[Serializable]
public class SerializedApple
{
	public bool onTree;
	public SerializableVector3 treeBase;
	public SerializableVector3 positon;
}