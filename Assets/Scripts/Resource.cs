using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Resource : ScriptableObject {
	public enum ResourceType {Other, Tree, Stone, Ore, Ground}
	public GameObject prefab;
	public ResourceType type;
	public int id;
	public Item[] resourceItems;
	public float[] chances;
	public int maxGathers;
	public float gatherTime;
	public float toughness;
	public bool infiniteGathers = false;
}
