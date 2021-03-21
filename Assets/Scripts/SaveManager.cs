using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour {

	PersistentData persistentData;

	[SerializeField] Animator canvasAnim;

	public Text saveText;
	public Item[] allItems;
	public Resource[] allResources;
	[SerializeField] private GameObject smallIslandPrefab;
	[SerializeField] private GameObject smallDarkIslandPrefab;
	[SerializeField] GameObject meteorPrefab;
	[SerializeField] GameObject applePrefab;

	Inventory inventory;
	PlayerController player;

	AchievementManager achievementManager;

	WorldManager worldManager;

	StructurePositionReferences StructurePosRef; //idk... WIP

	HiveMind hive;

	INeedModifier[] modifierAffected;

	public int difficulty;
	public int mode;
	public string modifier;

	FileInfo[] info;

	bool autoSave = true;
	float autoSaveInterval = 120f;
	float autoSaveTimer = 0f;

	void Awake() {
		modifierAffected = FindInterface<INeedModifier>().ToArray();
		hive = FindObjectOfType<HiveMind>();
		GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
		worldManager = FindObjectOfType<WorldManager>();

		achievementManager = FindObjectOfType<AchievementManager>();
		inventory = playerObj.GetComponent<Inventory>();
		player = playerObj.GetComponent<PlayerController>();
		persistentData = FindObjectOfType<PersistentData>();
		StructurePosRef = FindObjectOfType<StructurePositionReferences>();
	}

	void Start() {
		CheckSaveDirectory();
		DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath + "/saves");
		info = dir.GetFiles("*.*");
		if(persistentData) {
			if(persistentData.loadSave) {
				LoadGame(persistentData.saveToLoad);
			} else {
				difficulty = persistentData.difficulty;
				mode = persistentData.mode;
				modifier = persistentData.gameModifier;
				SaveGame();
				LoadStartingModifierObjects();
			}
			if(mode == 1) { // Creative mode
				inventory.LoadCreativeMode();
				player.LoadCreativeMode();
			}
		}
		if(autoSave) {
			autoSaveTimer = autoSaveInterval;
		}
	}

	void Update() {
		if(autoSave) {
			autoSaveTimer -= Time.deltaTime;
			if(autoSaveTimer <= 0f) {
				SaveGame();
				autoSaveTimer = autoSaveInterval;
			}
		}
	}

	Item FindItem(int id) {
		foreach(Item item in allItems) {
			if(item.id == id) {
				return item;
			}
		}
		Debug.LogError("Item with id " + id + " not found.");
		return null;
	}

	List<Item> IDsToItems(List<int> IDs) {
		List<Item> items = new List<Item>();
		foreach(int itemID in IDs) {
			items.Add(FindItem(itemID));
		}
		return items;
	}

	List<int> ItemsToIDs(List<Item> items) {
		List<int> IDs = new List<int>();
		foreach(Item item in items) {
			IDs.Add(item.id);
		}
		return IDs;
	}

	void CheckSaveDirectory() {
		if(!Directory.Exists(Application.persistentDataPath + "/saves")) {
			Directory.CreateDirectory(Application.persistentDataPath + "/saves");
		}
	}

	public void LoadGame(int saveNum) {
		CheckSaveDirectory();
		if(File.Exists(Application.persistentDataPath + "/saves/" + info[saveNum].Name)) {
			ClearWorld();

			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/saves/" + info[saveNum].Name, FileMode.Open);
			Save save = (Save)bf.Deserialize(file);
			file.Close();

			for(int i = 0; i < save.worldItemIDs.Count; i++) {
				foreach(Item item in allItems) {
					if(item.id == save.worldItemIDs[i]) {
						//GameObject itemObj = Instantiate(item.prefab, save.worldItemPositions[i], save.worldItemRotations[i]) as GameObject;
						GameObject itemObj = Instantiate(item.prefab, save.worldItemPositions[i], save.worldItemRotations[i]) as GameObject;
						if (!save.worldItemHasRigidbodies[i]) {
							Rigidbody itemRB = itemObj.GetComponentInParent<Rigidbody>();
							if(itemRB) {
								Destroy(itemRB);
							}
						}
						ItemHandler handler = itemObj.GetComponent<ItemHandler>();
						AutoMiner autoMiner = itemObj.GetComponent<AutoMiner>();
						AutoSorter autoSorter = itemObj.GetComponent<AutoSorter>();
						Radio radio = itemObj.GetComponent<Radio>();
						LightItem li = itemObj.GetComponent<LightItem>();
						GridPosition gr = itemObj.GetComponent<GridPosition>();
						ElevatorBlock elevBlock = itemObj.GetComponent<ElevatorBlock>();
						PistonLauncher launcher = itemObj.GetComponent<PistonLauncher>();

						if(handler.item.type == Item.ItemType.Structure && handler.item.subType == Item.ItemSubType.Furnace) {
							Furnace furnace = itemObj.GetComponent<Furnace>();
							furnace.fuel = save.itemSaveData[i].fuel;
							if(save.itemSaveData[i].itemID != -1) {
								foreach (int furnaceItem in save.itemSaveData[i].itemIDs) {
									furnace.currentSmeltingItem.Add(ItemIDToItem(furnaceItem)); //this could get VERY inefficient.
									furnace.finishTime = furnace.smeltTime + Time.time;
								}
							}
						}
						if (handler.item.type == Item.ItemType.Structure && handler.item.subType == Item.ItemSubType.Storage) //This is only temporary for the current storage solution!!
						{
							ItemHolder holder = handler.GetComponent<ItemHolder>();
							if (save.itemSaveData[i].itemAmount > 0)
							{
								holder.LoadValues(save.itemSaveData[i].itemAmount, ItemIDToItem(save.itemSaveData[i].itemID));
							}
							else
							{
								holder.LoadNullValues();
							}
						}
						if (launcher)
                        {
							launcher.LoadInternalForce(save.itemSaveData[i].num);
							launcher.LoadAutomationState(save.itemSaveData[i].bit);
                        }
						if (autoMiner) {
							autoMiner.items = IDsToItems(save.itemSaveData[i].itemIDs);
							autoMiner.itemAmounts = save.itemSaveData[i].itemAmounts;
							if(save.itemSaveData[i].itemID != -1) {
								autoMiner.SetTool(FindItem(save.itemSaveData[i].itemID));
							}
						}
						if(autoSorter) {
							if(save.itemSaveData[i].itemID != -1) {
								autoSorter.SetItem(FindItem(save.itemSaveData[i].itemID));
							}
						}
						if(handler.item.type == Item.ItemType.Structure && handler.item.subType == Item.ItemSubType.Conveyor) {
							ConveyorBelt conveyerBelt = itemObj.GetComponent<ConveyorBelt>();
							conveyerBelt.SetSpeed(save.itemSaveData[i].num);
						}
						if(radio) {
							radio.SetSong(save.itemSaveData[i].num);
						}
						if(li) {
							li.SetIntensity(save.itemSaveData[i].num);
						}
						if (gr)
                        {
							gr.Setup();
                        }
						if (elevBlock)
                        {
							elevBlock.SetPosRef(StructurePosRef);
                        }
					}
				}
			}

			if (save.meteorSaveData != null)
			{
				for (int i = 0; i < save.meteorSaveData.Count; i++)
				{
					MeteorSaveData msd = save.meteorSaveData[i];
					GameObject meteor = Instantiate(meteorPrefab, msd.pos, Quaternion.identity) as GameObject;
					meteor.GetComponent<Meteorite>().LoadValues(msd.path, msd.size, msd.speed, msd.penetrationTicks);
				}
			}

			for(int i = 0; i < save.worldResourceIDs.Count; i++) {
				foreach(Resource resource in allResources) {
					if(resource.id == save.worldResourceIDs[i]) {
						if(resource.prefab) {
							GameObject resourceObj = Instantiate(resource.prefab, save.worldResourcePositions[i], save.worldResourceRotations[i]) as GameObject;
							ResourceHandler handler = resourceObj.GetComponent<ResourceHandler>();
							if(handler) {
								hive.AddResource(handler);
							}
							resourceObj.GetComponent<ResourceHandler>().health = save.worldResourceHealths[i];
							TreeResource tree = resourceObj.GetComponent<TreeResource>();
							if(tree) {
								tree.spawnApples = false;
							}
						}
					}
				}
			}

			for(int i = 0; i < save.inventoryItemIDs.Count; i++) {
				foreach(Item item in allItems) {
					if(item.id == save.inventoryItemIDs[i]) {
						inventory.SetItem(item, save.inventoryItemAmounts[i], i);
					}
				}
			}

			if (save.apples != null)
			{
				for (int i = 0; i < save.apples.Count; i++)
				{
					GameObject newApple = Instantiate(applePrefab, save.apples[i].positon, Quaternion.identity);
					Apple appleComp = newApple.GetComponent<Apple>();
					if (save.apples[i].onTree)
					{
						appleComp.Attach(save.apples[i].treeBase);
					}
				}
			}

			foreach (Vector3 pos in save.smallIslandPositions) {
				Instantiate(smallIslandPrefab, pos, smallIslandPrefab.transform.rotation);
			}
			foreach (Vector3 pos in save.smallDarkIslandPositions)
            {
				Instantiate(smallDarkIslandPrefab, pos, smallDarkIslandPrefab.transform.rotation);
            }
			foreach (INeedModifier inm in modifierAffected)
            {
				inm.TakeModifier(save.modifier);
            }

			

			player.transform.position = save.playerPosition;
			player.transform.rotation = save.playerRotation;
			player.hunger = save.playerHunger;
			player.health = save.playerHealth;

			if(save.playerDead) {
				player.Die();
			} else {
				if(save.worldType == 0) {
					player.LoadLightWorld();
				} else {
					player.LoadDarkWorld();
				}
			}

			player.rb.velocity = save.playerVelocity;

			difficulty = save.difficulty;
			mode = save.mode;
			modifier = save.modifier;

			achievementManager.SetAchievements(save.achievementIDs);

			inventory.InventoryUpdate();

			saveText.text = "Game loaded from " + save.saveTime.ToString("HH:mm MMMM dd, yyyy");
		} else {
			saveText.text = "No game save found";
		}
	}

	public void LoadStartingModifierObjects ()
    {
		foreach (INeedModifier inm in modifierAffected)
		{
			inm.TakeModifier(persistentData.gameModifier);
		}
	}

	public void SaveGame() {
		canvasAnim.SetTrigger("Save");
		CheckSaveDirectory();
		Save save = CreateSave();

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file;
		if(persistentData.loadSave) {
			file = File.Create(Application.persistentDataPath + "/saves/" + info[persistentData.saveToLoad].Name);
		} else {
			file = File.Create(Application.persistentDataPath + "/saves/" + persistentData.newSaveName + ".save");
		}
		bf.Serialize(file, save);
		file.Close();

		saveText.text = "Game saved at " + DateTime.Now.ToString("HH:mm MMMM dd, yyyy");
	}

	private Save CreateSave() {
		Save save = new Save();

		save.playerPosition = player.transform.position;
		save.playerRotation = player.transform.rotation;
		save.playerHealth = player.health;
		save.playerHunger = player.hunger;
		save.saveTime = DateTime.Now;

		if(player.currentWorld == WorldManager.WorldType.Light) {
			save.worldType = 0;
		} else {
			save.worldType = 1;
		}

		foreach(InventorySlot slot in inventory.slots) {
			if(slot.currentItem) {
				save.inventoryItemIDs.Add(slot.currentItem.id);
				save.inventoryItemAmounts.Add(slot.stackCount);
			} else {
				save.inventoryItemIDs.Add(-1);
				save.inventoryItemAmounts.Add(-1);
			}
		}

		List<ItemHandler> usedItemHandlers = new List<ItemHandler>();

		foreach(GameObject itemObj in GameObject.FindGameObjectsWithTag("Item")) {
			ItemHandler handler = itemObj.GetComponentInParent<ItemHandler>();
			if(!usedItemHandlers.Contains(handler)) {
				save.worldItemIDs.Add(handler.item.id);
				// Saving Item Data
				ItemSaveData itemSaveData = new ItemSaveData(); // Better way to do this would be to check item ids. //Micro here: it would be better but not that much better
				AutoMiner autoMiner = handler.GetComponent<AutoMiner>();
				AutoSorter autoSorter = handler.GetComponent<AutoSorter>();
				Radio radio = handler.GetComponent<Radio>();
				LightItem li = handler.GetComponent<LightItem>();
				PistonLauncher pl = handler.GetComponent<PistonLauncher>();

				if(handler.item.type == Item.ItemType.Structure && handler.item.subType == Item.ItemSubType.Furnace) { //furnaces
					Furnace furnace = handler.GetComponent<Furnace>();
					itemSaveData.fuel = furnace.fuel;
					if(furnace.currentSmeltingItem.Count > 0) {
						foreach (Item fItem in furnace.currentSmeltingItem) {
							itemSaveData.itemIDs.Add(fItem.id);
						}
					} else {
						itemSaveData.itemID = -1;
					}
				}
				if (handler.item.type == Item.ItemType.Structure && handler.item.subType == Item.ItemSubType.Storage) //This is only temporary for the current storage solution!!
				{ 
					ItemHolder holder = handler.GetComponent<ItemHolder>();
					if (holder.ItemStack() > 0)
					{
						itemSaveData.itemID = holder.GetItem().id;
						itemSaveData.itemAmount = holder.ItemStack();
					}
					else
					{
						itemSaveData.itemID = -1;
						itemSaveData.itemAmount = 0;
					}
				}

				if (pl)
				{
					itemSaveData.num = pl.GetInternalForce();
					itemSaveData.bit = pl.GetAutomationState();
				}

				if (autoMiner) {
					itemSaveData.itemIDs = ItemsToIDs(autoMiner.items);
					itemSaveData.itemAmounts = autoMiner.itemAmounts;
					if(autoMiner.currentToolItem) {
						itemSaveData.itemID = autoMiner.currentToolItem.id;
					} else {
						itemSaveData.itemID = -1;
					}
				}
				if(autoSorter) {
					if(autoSorter.sortingItem) {
						itemSaveData.itemID = autoSorter.sortingItem.id;
					} else {
						itemSaveData.itemID = -1;
					}
				}
				if(handler.item.type == Item.ItemType.Structure && handler.item.subType == Item.ItemSubType.Conveyor) { //conveyors
					ConveyorBelt conveyerBelt = handler.GetComponent<ConveyorBelt>();
					itemSaveData.num = conveyerBelt.speedNum;
				}
				if(radio) {
					itemSaveData.num = radio.songNum;
				}
				if(li) {
					itemSaveData.num = li.intensityNum;
				}
				/*
				if(handler.item.id == 26) { // Apple
					apples.Add(handler.gameObject);
					save.treeAssociation.Add(-1);
				} else {
					save.treeAssociation.Add(-2);
				}
				*/
				//yep, an exception for apples...
				/*Apple apple = itemObj.GetComponent<Apple>();
				if (apple)
                {
					if (!apple.Attached())
                    {
						save.itemSaveData.Add(itemSaveData);
					} else
                    {
						SerializedApple sapple = new SerializedApple();
						sapple.onTree = apple.Attached();
						sapple.treeBase = apple.GetTreeBase();
						sapple.positon = apple.gameObject.transform.position;
						save.apples.Add(sapple);
                    }
                } else
                {
					save.itemSaveData.Add(itemSaveData);
				}*/ //may be revisited later
				save.itemSaveData.Add(itemSaveData);
				// Done saving item data
				save.worldItemPositions.Add(itemObj.transform.position);
				save.worldItemRotations.Add(itemObj.transform.rotation);
				Rigidbody itemRB = itemObj.GetComponentInParent<Rigidbody>();
				save.worldItemHasRigidbodies.Add(itemRB);
				usedItemHandlers.Add(handler);
			}
		}

		foreach (Meteorite meteor in FindObjectsOfType<Meteorite>())
        {
			MeteorSaveData meteorSave = new MeteorSaveData();
			meteorSave.path = meteor.GetPath();
			meteorSave.size = meteor.GetSize();
			meteorSave.speed = meteor.GetSpeed();
			meteorSave.penetrationTicks = meteor.GetPticks();
			meteorSave.pos = meteor.gameObject.transform.position;
			save.meteorSaveData.Add(meteorSave);
		}

		List<ResourceHandler> usedResourceHandlers = new List<ResourceHandler>();

		foreach(GameObject resourceObj in GameObject.FindGameObjectsWithTag("Resource")) {
			ResourceHandler handler = resourceObj.GetComponentInParent<ResourceHandler>();
			if(!usedResourceHandlers.Contains(handler)) {
				save.worldResourceIDs.Add(handler.resource.id);
				save.worldResourcePositions.Add(resourceObj.transform.position);
				save.worldResourceRotations.Add(resourceObj.transform.rotation);
				save.worldResourceHealths.Add(handler.health);
				/*
				if(handler.resource.id == 5) { // Tree
					TreeResource tr = handler.GetComponent<TreeResource>();
					if(tr.apples.Count >= 1) {
						// AHH DO NOT LET THE APPLES FALL OUT OF THE TREE
					}
				}
				*/
				usedResourceHandlers.Add(handler);
			}
		}

		save.achievementIDs = achievementManager.ObtainedAchievements();

		foreach(SmallIsland si in FindObjectsOfType<SmallIsland>()) {
			if (si.islandType == SmallIsland.IslandType.Light)
			{
				save.smallIslandPositions.Add(si.transform.position);
			} else if (si.islandType == SmallIsland.IslandType.Dark)
            {
				save.smallDarkIslandPositions.Add(si.transform.position);
			}
		}
		

		save.difficulty = difficulty;
		save.mode = mode;
		save.modifier = modifier;

		save.playerDead = player.dead;

		save.playerVelocity = player.rb.velocity;

		return save;
	}

	void ClearWorld() {
		foreach(GameObject itemObj in GameObject.FindGameObjectsWithTag("Item")) {
			Destroy(itemObj);
		}

		foreach(GameObject resourceObj in GameObject.FindGameObjectsWithTag("Resource")) {
			if(resourceObj.GetComponent<ResourceHandler>().resource.prefab) {
				Destroy(resourceObj);
			}
		}

		foreach(InventorySlot slot in inventory.slots) {
			slot.ClearItem();
		}
	}

	public Item ItemIDToItem (int ID)
    {
		Item returnedItem = new Item();
		for (int i = 0; i < allItems.Length; i++)
        {
			if (allItems[i].id == ID)
            {
				returnedItem = allItems[i];
            }
        }
		return returnedItem;
    }

	public static List<T> FindInterface<T>()
	{
		List<T> interfaces = new List<T>();
		GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
		foreach (var rootGameObject in rootGameObjects)
		{
			T[] childrenInterfaces = rootGameObject.GetComponentsInChildren<T>();
			foreach (var childInterface in childrenInterfaces)
			{
				interfaces.Add(childInterface);
			}
		}
		return interfaces;
	}
}


[System.Serializable]
public struct SerializableVector3 {

	public float x;
	public float y;
	public float z;

	public SerializableVector3(float rX, float rY, float rZ) {
		x = rX;
		y = rY;
		z = rZ;
	}

	public override string ToString() {
		return String.Format("[{0}, {1}, {2}]", x, y, z);
	}

	public static implicit operator Vector3(SerializableVector3 rValue) {
		return new Vector3(rValue.x, rValue.y, rValue.z);
	}

	public static implicit operator SerializableVector3(Vector3 rValue) {
		return new SerializableVector3(rValue.x, rValue.y, rValue.z);
	}
}

[System.Serializable]
public struct SerializableQuaternion {

	public float x;
	public float y;
	public float z;
	public float w;

	public SerializableQuaternion(float rX, float rY, float rZ, float rW) {
		x = rX;
		y = rY;
		z = rZ;
		w = rW;
	}

	public override string ToString() {
		return String.Format("[{0}, {1}, {2}, {3}]", x, y, z, w);
	}

	public static implicit operator Quaternion(SerializableQuaternion rValue) {
		return new Quaternion(rValue.x, rValue.y, rValue.z, rValue.w);
	}

	public static implicit operator SerializableQuaternion(Quaternion rValue) {
		return new SerializableQuaternion(rValue.x, rValue.y, rValue.z, rValue.w);
	}
}

//https://answers.unity.com/questions/863509/how-can-i-find-all-objects-that-have-a-script-that.html
// https://answers.unity.com/questions/956047/serialize-quaternion-or-vector3.html