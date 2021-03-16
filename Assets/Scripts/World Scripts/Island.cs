using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Island
{
	

	public WorldManager.WorldType worldType;

	public bool spawnSmallIslands = true;
	public bool useIslandCrates;
	public GameObject smallIslandPrefab;

	public GameObject island;
	public GameObject starterCratePrefab;
	public GameObject cratePrefab;

	public float bounds;

	public Vector3[] smallIslandSpawnLocations;
	//public GameObject copperOreResourcePrefab;
	public WeightedSpawnableItem[] spawns;
	public WeightedSpawnableItem[] grasstypeSpawns;
	public float initialSpawnAttempts;
	public float totalWeight; //the amount of all weights, combined (hopefully brings efficiency to spawning)
	public float[] weightStarts; //EG weight for stone is 40, tree is 20, grass is 10, this will be 0, 40, 60
	public float grassTotalWeight;
	public float[] grassWeightStarts;
	public float[] spawnTimes;

	public float[] nextSpawnTime;

	public float smallIslandSpawnTime = 60f;
	public float nextSmallIslandSpawnTime;
}
