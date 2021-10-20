using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Resource : ScriptableObject {
	public enum ResourceType {Other, Tree, Stone, Ore, Ground}
	public GameObject prefab; //This needs to be set if you have a standalone resource, eg stone or trees. If you have multiple standalone resources sharing the same prefab... too bad i guess
	public ResourceType type;
	public int id;
	public Item[] resourceItems;
	public float[] chances;
	public int maxGathers;
	public float gatherTime;
	public float toughness;
	public bool infiniteGathers = false;
}
