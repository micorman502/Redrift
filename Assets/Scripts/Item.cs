using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject {
	public enum ItemType {Resource, Tool, Structure, Food, Weapon}
	public enum ItemSubType {Base, Furnace, Conveyor, Storage} //Later use a custom editor to make subtypes make more sense
	public string itemName;
	public string itemDescription;
	public ItemType type;
	public ItemSubType subType;
	public int id;
	public Sprite icon;
	public GameObject prefab;
	public float timeToGather = 0.25f;
	public Vector3 handRotation;
	public Vector3 handScale = Vector3.one;
	public int maxStackCount;
	public GameObject previewPrefab;
	public float speed;
	public int gatherAmount;
	public float toolToughness; //if tool tougness >= resource toughness, you can gather that resource.
	public float power;
	public float fuel;
	public Item smeltItem;
	public bool alignToNormal; // for buildings. I assume false if it works like  grass and flowers.
	public Vector3 gridSize;
	public Vector3[] rots;
	public int achievementNumber = -1;
	public float calories;
}
