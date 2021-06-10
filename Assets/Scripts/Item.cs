using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject {
	public enum ItemType {Resource, Tool, Structure, Food, Weapon}
	public enum ItemSubType {Base, Furnace, Conveyor, Storage, Incinerator} //Later use a custom editor to make subtypes make more sense
	public string itemName;
	public string itemDescription;
	public int id;
	public int maxStackCount;
	public ItemType type;
	public ItemSubType subType;
	public Sprite icon;
	public GameObject prefab;
	public GameObject previewPrefab;
	public GameObject handPrefab;
	public float timeToGather = 0.25f;
	public Vector3 handRotation;
	public Vector3 handScale = Vector3.one;
	public float speed;
	public int gatherAmount;
	public float toolToughness; //if tool tougness >= resource toughness, you can gather that resource.
	public float power;
	public float fuel;
	public Item smeltItem;
	public bool alignToNormal; // for buildings. false if it works like  grass and flowers.
	public Vector3 gridSize;
	public Vector3[] rots;
	public int achievementNumber = -1;
	public float calories;
	public float boringFoodPenalty; //the penalty, in calories, for eating this item more than once in a row.
}
