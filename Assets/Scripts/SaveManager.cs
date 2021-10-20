using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class SaveManager : MonoBehaviour {
	public static SaveManager Instance;
	public GameObject[] loadedObjects;
	PersistentData persistentData;

	[SerializeField] Animator canvasAnim;

	public Text saveText;
	public Item[] allItems;
	public Resource[] allResources;
	[SerializeField] GameObject smallIslandPrefab;

	Inventory inventory;
	PlayerController player;

	WorldManager worldManager;

	public int difficulty;
	public int mode;

	FileInfo[] info;

	bool autoSave = true;
	float autoSaveInterval = 120f;
	float autoSaveTimer = 0f;

	void Awake() {
		if (Instance)
        {
			Destroy(this);
			return;
        }
		Instance = this;
		GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
		worldManager = FindObjectOfType<WorldManager>();

		inventory = playerObj.GetComponent<Inventory>();
		player = playerObj.GetComponent<PlayerController>();
		persistentData = FindObjectOfType<PersistentData>();
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
				SaveGame();
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

	public Item FindItem(int id) {
		foreach(Item item in allItems) {
			if(item.id == id) {
				return item;
			}
		}
		Debug.LogError("Item with id " + id + " not found.");
		return null;
	}

	public List<Item> IDsToItems(List<int> IDs) {
		List<Item> items = new List<Item>();
		foreach(int itemID in IDs) {
			items.Add(FindItem(itemID));
		}
		return items;
	}

	public List<int> ItemsToIDs(List<Item> items) {
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
		string path = Application.persistentDataPath + "/saves/" + info[saveNum].Name;
		if (File.Exists(path)) {
			ClearWorld();

			Save save = JsonConvert.DeserializeObject<Save>(File.ReadAllText(path));

			for (int i = 0; i < save.savedObjects.Count; i++)
            {
				ObjectSaveData newObjData = save.savedObjects[i];
				ItemSaveData newData = save.savedObjectsInfo[i];
				GameObject newObj = Instantiate(loadedObjects[newObjData.objectID], newObjData.position, newObjData.rotation);
				IItemSaveable[] saveables = newObj.GetComponents<IItemSaveable>();
				for (int s = 0; s < saveables.Length; s++)
				{
					saveables[s].SetData(newData, newObjData);
				}
            }

			for (int i = 0; i < save.inventoryItems.Count; i++) {
				foreach(Item item in allItems) {
					if(item.id == save.inventoryItems[i].id) {
						inventory.SetItem(item, save.inventoryItems[i].amount, i);
					}
				}
			}


			player.transform.position = save.playerTransform.position;
			player.transform.rotation = save.playerTransform.rotation;
			player.hunger = save.playerHunger;
			player.health = save.playerHealth;
			if (save.playerDead) {
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

			AchievementManager.Instance.SetAchievements(save.achievementIDs);

			inventory.InventoryUpdate();

			saveText.text = "Game loaded from " + save.saveTime.ToString("HH:mm MMMM dd, yyyy");
		} else {
			saveText.text = "No game save found";
		}
	}

	public void SaveGame() {
		canvasAnim.SetTrigger("Save");
		CheckSaveDirectory();
		Save save = CreateSave();

		string path;
		if(persistentData.loadSave) {
			path = Application.persistentDataPath + "/saves/" + info[persistentData.saveToLoad].Name;
		} else {
			path = Application.persistentDataPath + "/saves/" + persistentData.newSaveName + ".save";
		}
		File.WriteAllText(path, JsonConvert.SerializeObject(save));

		saveText.text = "Game saved at " + DateTime.Now.ToString("HH:mm MMMM dd, yyyy");
	}

	private Save CreateSave() {
		Save save = new Save();

		save.playerTransform = new ObjectSaveData(player.transform.position, player.transform.rotation, 0);
		save.playerHealth = player.health;
		save.playerHunger = player.hunger;
		save.saveTime = DateTime.Now;

		if(player.currentWorld == WorldManager.WorldType.Light) {
			save.worldType = 0;
		} else {
			save.worldType = 1;
		}

		foreach(WorldItem item in inventory.GetInventory()) {
			if(item.item) {
				save.inventoryItems.Add(item);
			} else {
				save.inventoryItems.Add(new SerializedWorldItem(-1, -1));
			}
		}

		foreach (IItemSaveable saveable in FindInterfaces.Find<IItemSaveable>())
        {
			saveable.GetData(out ItemSaveData data, out ObjectSaveData objData, out bool dontSave);
			if (dontSave || objData.objectID == -1)
				continue;
			save.savedObjects.Add(objData);
			save.savedObjectsInfo.Add(data);
		}

		save.achievementIDs = AchievementManager.Instance.ObtainedAchievements();

		save.difficulty = difficulty;
		save.mode = mode;

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
		inventory.ClearInventory();
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

// https://answers.unity.com/questions/956047/serialize-quaternion-or-vector3.html