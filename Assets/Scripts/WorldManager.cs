using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeightedSpawnableItem
{
	public GameObject spawn;
	public int weight; //higher weight means higher spawn chance
}

public class WorldManager : MonoBehaviour {

	public enum WorldType {Light, Dark}

	public WorldType worldType;

	public bool spawnSmallIslands = true;
	public bool useIslandCrates;
	public GameObject smallIslandPrefab;

	public GameObject world;
	public GameObject starterCratePrefab;
	public GameObject cratePrefab;

	public float bounds;

	public Vector3[] smallIslandSpawnLocations;
	//public GameObject copperOreResourcePrefab;
	public WeightedSpawnableItem[] spawns;
	public WeightedSpawnableItem[] grasstypeSpawns;
	public float initialSpawnAttempts;
	private float totalWeight; //the amount of all weights, combined (hopefully brings efficiency to spawning)
	private float[] weightStarts; //EG weight for stone is 40, tree is 20, grass is 10, this will be 0, 40, 60
	private float grassTotalWeight; 
	private float[] grassWeightStarts; 
	public float[] spawnTimes;

	public HiveMind hive;

	float[] nextSpawnTime;

	public float smallIslandSpawnTime = 60f;
	float nextSmallIslandSpawnTime;

	int difficulty;

	bool gameStarted;

	PersistentData persistentData;

	void Start() {
		SetUpWeights();
		persistentData = FindObjectOfType<PersistentData>();
		if(persistentData) {
			difficulty = persistentData.difficulty;
			if(!persistentData.loadSave) {
				GenerateWorld();
			}
		} else {
			GenerateWorld();
		}
		SetUpWorld();
		for (int i = 0; i < Mathf.RoundToInt(initialSpawnAttempts * 4); i++)
		{
			SpawnRandomWeightedGrassObjects();
		}
	}

	void SetUpWorld() {
		nextSpawnTime = new float[spawnTimes.Length];

		int c = 0;
		foreach(float spawnTime in spawnTimes) {
			nextSpawnTime[c] = Time.time + spawnTimes[c];
			c++;
		}

		if(spawnSmallIslands) {
			nextSmallIslandSpawnTime = Time.time + smallIslandSpawnTime;
			SpawnSmallIsland();
		}
	}

	void SetUpWeights ()
    {
		int tempTotalWeight = 0;
		weightStarts = new float[spawns.Length];
		for (int i = 0; i < spawns.Length; i++)
		{
			tempTotalWeight += spawns[i].weight;
			if (i == 0)
			{
				weightStarts[i] = 0;
			}
			else
			{
				weightStarts[i] = weightStarts[i - 1] + spawns[i - 1].weight;
			}
		}
		totalWeight = tempTotalWeight;

		int tempTotalGrassWeight = 0;
		grassWeightStarts = new float[grasstypeSpawns.Length];
		for (int i = 0; i < grasstypeSpawns.Length; i++)
		{
			tempTotalWeight += grasstypeSpawns[i].weight;
			if (i == 0)
			{
				grassWeightStarts[i] = 0;
			}
			else
			{
				grassWeightStarts[i] = grassWeightStarts[i - 1] + grasstypeSpawns[i - 1].weight;
			}
		}
		grassTotalWeight = tempTotalGrassWeight;
	}

	void GenerateWorld() {

		for (int i = 0; i < initialSpawnAttempts; i++)
        {
			SpawnRandomWeightedObject();
        }

		if(difficulty == 0 && worldType == WorldType.Light) {
			for(int d = 0; d < 2; d++) {
				Instantiate(starterCratePrefab, Vector3.up * 4f + Vector3.up * d, starterCratePrefab.transform.rotation);
			}
		}
	}

	void Update() {
		int i = 0;
		foreach(float spawnTime in nextSpawnTime) {
			if(Time.time >= spawnTime) {
				SpawnRandomWeightedObject();
				nextSpawnTime[i] = Time.time + spawnTimes[i];
			}
			i++;
		}

		if(spawnSmallIslands) {
			if(Time.time >= nextSmallIslandSpawnTime) {
				SpawnSmallIsland();
				nextSmallIslandSpawnTime = Time.time + smallIslandSpawnTime;
			}
		}
	}

	public void SpawnObject (int spawnIndex)
    {
		Vector3 pos = transform.TransformPoint(new Vector3(Random.insideUnitCircle.x * bounds, 300f, Random.insideUnitCircle.y * bounds));
		RaycastHit hit;
		if (Physics.Raycast(pos, Vector3.down, out hit, Mathf.Infinity, -1))
		{
			if (hit.collider.gameObject == world)
			{
				GameObject obj = Instantiate(spawns[spawnIndex].spawn, hit.point, Quaternion.LookRotation(hit.normal)) as GameObject;
				hive.AddResource(obj.GetComponent<ResourceHandler>());
				obj.transform.Rotate(Vector3.forward * Random.Range(0f, 360f));
			}
		}
	}

	public void SpawnRandomObject ()
    {
		Vector3 pos = transform.TransformPoint(new Vector3(Random.insideUnitCircle.x * bounds, 300f, Random.insideUnitCircle.y * bounds));
		RaycastHit hit;
		if (Physics.Raycast(pos, Vector3.down, out hit, Mathf.Infinity, -1))
		{
			if (hit.collider.gameObject == world)
			{
				GameObject obj = Instantiate(spawns[Random.Range(0, spawns.Length)].spawn, hit.point, Quaternion.LookRotation(hit.normal)) as GameObject;
				hive.AddResource(obj.GetComponent<ResourceHandler>());
				obj.transform.Rotate(Vector3.forward * Random.Range(0f, 360f));
			}
		}
	}

	public void SpawnRandomWeightedObject ()
    {
		bool siSuccess = false; //spawn index success.
		//i'm sure this can be more efficient but hopefully it works for now
		int thisWeight = Mathf.RoundToInt(Random.Range(0, totalWeight));
		int spawnIndex = 0;
		for (int l = 0; l < 1000; l++)
		{
			for (int i = 0; i < weightStarts.Length; i++)
			{
				if (thisWeight >= weightStarts[i] && i < weightStarts.Length - 1 && thisWeight < weightStarts[i + 1])
				{
					spawnIndex = i;
					siSuccess = true;
					break;
				}
				else if (i == weightStarts.Length - 1) //the "thisWeight < weightStarts[i+1]" causes issues if i = weightStarts.Length or something like that
				{
					spawnIndex = i;
					siSuccess = true;
					break;
				}
			}
			if (siSuccess == true)
			{
				break;
			} else
            {
				thisWeight = Mathf.RoundToInt(Random.Range(0, totalWeight));
			}
		}
		Vector3 pos = transform.TransformPoint(new Vector3(Random.insideUnitCircle.x * bounds, 300f, Random.insideUnitCircle.y * bounds));
		RaycastHit hit;
		if (Physics.Raycast(pos, Vector3.down, out hit, Mathf.Infinity, -1))
		{
			if (hit.collider.gameObject == world)
			{
				GameObject obj = Instantiate(spawns[spawnIndex].spawn, hit.point, Quaternion.LookRotation(hit.normal)) as GameObject;
				hive.AddResource(obj.GetComponent<ResourceHandler>());
				obj.transform.Rotate(Vector3.forward * Random.Range(0f, 360f));
			}
		}
	}

	public void SpawnRandomWeightedGrassObjects()
	{
		bool siSuccess = false; //spawn index success.
								//i'm sure this can be more efficient but hopefully it works for now
		int thisWeight = Mathf.RoundToInt(Random.Range(0, totalWeight));
		int spawnIndex = 0;
		for (int l = 0; l < 1000; l++)
		{
			for (int i = 0; i < weightStarts.Length; i++)
			{
				if (thisWeight >= grassWeightStarts[i] && i < grassWeightStarts.Length - 1 && thisWeight < grassWeightStarts[i + 1])
				{
					spawnIndex = i;
					siSuccess = true;
					break;
				}
				else if (i == grassWeightStarts.Length - 1) //the "thisWeight < weightStarts[i+1]" causes issues if i = weightStarts.Length or something like that
				{
					spawnIndex = i;
					siSuccess = true;
					break;
				}
			}
			if (siSuccess == true)
			{
				break;
			}
			else
			{
				thisWeight = Mathf.RoundToInt(Random.Range(0, grassTotalWeight));
			}
		}
		Vector3 pos = transform.TransformPoint(new Vector3(Random.insideUnitCircle.x * bounds, 300f, Random.insideUnitCircle.y * bounds));
		RaycastHit hit;
		if (Physics.Raycast(pos, Vector3.down, out hit, Mathf.Infinity, -1))
		{
			if (hit.collider.gameObject == world)
			{
				GameObject obj = Instantiate(grasstypeSpawns[spawnIndex].spawn, hit.point, Quaternion.LookRotation(hit.normal)) as GameObject;
				obj.transform.Rotate(Vector3.forward * Random.Range(0f, 360f));
			}
		}
	}

	void SpawnSmallIsland() {
		GameObject islandObj = Instantiate(smallIslandPrefab, transform.TransformPoint(smallIslandSpawnLocations[Random.Range(0, smallIslandSpawnLocations.Length)]), smallIslandPrefab.transform.rotation) as GameObject;
		Vector3 pos = new Vector3(Random.Range(-1f, 1f), 300f, Random.Range(-1f, 1f)) + islandObj.transform.position;
		RaycastHit hit;
		if (useIslandCrates)
		{
			if (Physics.Raycast(pos, Vector3.down, out hit, Mathf.Infinity, -1))
			{
				GameObject crateObj = Instantiate(cratePrefab, hit.point + Vector3.up * 0.5f, Quaternion.LookRotation(hit.normal)) as GameObject;
				crateObj.transform.Rotate(Vector3.forward * Random.Range(0f, 360f));
			}
		}
		//if(Random.Range(0, 2) == 0) {
		//	if(Physics.Raycast(pos, Vector3.down, out hit, Mathf.Infinity, -1)) {
		//		GameObject oreObj = Instantiate(copperOreResourcePrefab, hit.point, Quaternion.LookRotation(hit.normal), islandObj.transform) as GameObject;
		//		oreObj.transform.Rotate(Vector3.forward * Random.Range(0f, 360f));
		//	}
		//} else {
			
		//}
	}
}
