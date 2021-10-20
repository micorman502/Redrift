using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Base", order = 0)]
public class ItemInfo : ScriptableObject
{
	public string itemName;
	public string itemDescription;
	public int stackSize;
	public int id;
	public Sprite icon;
	public GameObject droppedPrefab;
	public float timeToGather = 0.25f;
}
