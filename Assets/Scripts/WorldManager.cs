using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeightedSpawnableItem
{
	public enum SpawnLocation { Above, Below };
	public SpawnLocation spawnLocation;
	public GameObject spawn;
	public int weight; //higher weight means higher spawn chance
}

public class WorldManager : MonoBehaviour
{

	public enum WorldType { Light, Dark }
	public Island[] islands;

	public HiveMind hive;


	int difficulty;

	bool gameStarted;

	PersistentData persistentData;

	void Start()
	{
		SetUpWeights();
		persistentData = FindObjectOfType<PersistentData>();
		if (persistentData)
		{
			difficulty = persistentData.difficulty;
			if (!persistentData.loadSave)
			{
				GenerateWorld();
			}
		}
		else
		{
			GenerateWorld();
		}
		SetUpWorld();
		for (int isl = 0; isl < islands.Length; isl++)
		{
			if (islands[isl].grasstypeSpawns.Length > 0)
			{
				for (int i = 0; i < Mathf.RoundToInt(islands[isl].initialSpawnAttempts * 4); i++)
				{
					SpawnRandomWeightedGrassObjects(isl);
				}
			}
		}
	}

	void SetUpWorld()
	{
		for (int isl = 0; isl < islands.Length; isl++)
		{
			islands[isl].nextSpawnTime = new float[islands[isl].spawnTimes.Length];

			int c = 0;
			foreach (float spawnTime in islands[isl].spawnTimes)
			{
				islands[isl].nextSpawnTime[c] = Time.time + islands[isl].spawnTimes[c];
				c++;
			}

			if (islands[isl].spawnSmallIslands)
			{
				islands[isl].nextSmallIslandSpawnTime = Time.time + islands[isl].smallIslandSpawnTime;
				SpawnSmallIsland(isl);
			}
		}
	}

	void SetUpWeights()
	{
		for (int isl = 0; isl < islands.Length; isl++)
		{
			int tempTotalWeight = 0;
			islands[isl].weightStarts = new float[islands[isl].spawns.Length];
			for (int i = 0; i < islands[isl].spawns.Length; i++)
			{
				tempTotalWeight += islands[isl].spawns[i].weight;
				if (i == 0)
				{
					islands[isl].weightStarts[i] = 0;
				}
				else
				{
					islands[isl].weightStarts[i] = islands[isl].weightStarts[i - 1] + islands[isl].spawns[i - 1].weight;
				}
			}
			islands[isl].totalWeight = tempTotalWeight;

			int tempTotalGrassWeight = 0;
			islands[isl].grassWeightStarts = new float[islands[isl].grasstypeSpawns.Length];
			for (int i = 0; i < islands[isl].grasstypeSpawns.Length; i++)
			{
				tempTotalWeight += islands[isl].grasstypeSpawns[i].weight;
				if (i == 0)
				{
					islands[isl].grassWeightStarts[i] = 0;
				}
				else
				{
					islands[isl].grassWeightStarts[i] = islands[isl].grassWeightStarts[i - 1] + islands[isl].grasstypeSpawns[i - 1].weight;
				}
			}
			islands[isl].grassTotalWeight = tempTotalGrassWeight;
		}
	}

	void GenerateWorld()
	{
		for (int isl = 0; isl < islands.Length; isl++)
		{
			for (int i = 0; i < islands[isl].initialSpawnAttempts; i++)
			{
				SpawnRandomWeightedObject(isl);
			}

			if (difficulty == 0 && islands[isl].worldType == WorldType.Light)
			{
				for (int d = 0; d < 2; d++)
				{
					Instantiate(islands[isl].starterCratePrefab, Vector3.up * 4f + Vector3.up * d, islands[isl].starterCratePrefab.transform.rotation);
				}
			}
		}
	}

	void Update()
	{
		for (int isl = 0; isl < islands.Length; isl++)
		{
			int i = 0;
			foreach (float spawnTime in islands[isl].nextSpawnTime)
			{
				if (Time.time >= spawnTime)
				{
					SpawnRandomWeightedObject(isl);
					islands[isl].nextSpawnTime[i] = Time.time + islands[isl].spawnTimes[i];
				}
				i++;
			}

			if (islands[isl].spawnSmallIslands)
			{
				if (Time.time >= islands[isl].nextSmallIslandSpawnTime)
				{
					SpawnSmallIsland(isl);
					islands[isl].nextSmallIslandSpawnTime = Time.time + islands[isl].smallIslandSpawnTime;
				}
			}
		}
	}

	public void SpawnObject(int spawnIndex, int isl)
	{
		Vector3 pos = new Vector3();
		Vector3 islandPos = islands[isl].island.transform.position;
		Vector3 rayDir = new Vector3();
		RaycastHit hit;
		if (islands[isl].spawns[spawnIndex].spawnLocation == WeightedSpawnableItem.SpawnLocation.Above) //this determines where the object spawns, above or below
		{
			pos = transform.TransformPoint(new Vector3(Random.insideUnitCircle.x * islands[isl].bounds, 300f, Random.insideUnitCircle.y * islands[isl].bounds));
			rayDir = Vector3.down;
		}
		else if (islands[isl].spawns[spawnIndex].spawnLocation == WeightedSpawnableItem.SpawnLocation.Below)
		{
			pos = transform.TransformPoint(new Vector3(Random.insideUnitCircle.x * islands[isl].bounds, -300f, Random.insideUnitCircle.y * islands[isl].bounds));
			rayDir = Vector3.up;
		}
		Debug.DrawRay(pos, rayDir * 100f, Color.red, 100f);
		if (Physics.Raycast(islandPos + pos, rayDir, out hit, Mathf.Infinity, ~LayerMask.GetMask("Ignore Raycast")))
		{
			if (hit.collider.gameObject == islands[isl].island)
			{
				GameObject obj = Instantiate(islands[isl].spawns[spawnIndex].spawn, hit.point, Quaternion.LookRotation(hit.normal)) as GameObject;
				hive.AddResource(obj.GetComponent<ResourceHandler>());
				obj.transform.Rotate(Vector3.forward * Random.Range(0f, 360f));
			}
		}
	}

	public void SpawnRandomObject(int isl)
	{
		SpawnObject(Random.Range(0, islands[isl].spawns.Length - 1), isl);
	}

	public void SpawnRandomWeightedObject(int isl)
	{
		bool siSuccess = false; //spawn index success.
								//i'm sure this can be more efficient but hopefully it works for now
		int thisWeight = Mathf.RoundToInt(Random.Range(0, islands[isl].totalWeight));
		int spawnIndex = 0;
		for (int l = 0; l < 1000; l++)
		{
			for (int i = 0; i < islands[isl].weightStarts.Length; i++)
			{
				if (thisWeight >= islands[isl].weightStarts[i] && i < islands[isl].weightStarts.Length - 1 && thisWeight < islands[isl].weightStarts[i + 1])
				{
					spawnIndex = i;
					siSuccess = true;
					break;
				}
				else if (i == islands[isl].weightStarts.Length - 1) //the "thisWeight < weightStarts[i+1]" causes issues if i = weightStarts.Length or something like that
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
				thisWeight = Mathf.RoundToInt(Random.Range(0, islands[isl].totalWeight));
			}
		}
		SpawnObject(spawnIndex, isl);
	}

	public void SpawnRandomWeightedGrassObjects(int isl)
	{
		bool siSuccess = false; //spawn index success.
								//i'm sure this can be more efficient but hopefully it works for now
		int thisWeight = Mathf.RoundToInt(Random.Range(0, islands[isl].totalWeight));
		int spawnIndex = 0;
		for (int l = 0; l < 1000; l++)
		{
			for (int i = 0; i < islands[isl].weightStarts.Length; i++)
			{
				if (thisWeight >= islands[isl].grassWeightStarts[i] && i < islands[isl].grassWeightStarts.Length - 1 && thisWeight < islands[isl].grassWeightStarts[i + 1])
				{
					spawnIndex = i;
					siSuccess = true;
					break;
				}
				else if (i == islands[isl].grassWeightStarts.Length - 1) //the "thisWeight < weightStarts[i+1]" causes issues if i = weightStarts.Length or something like that
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
				thisWeight = Mathf.RoundToInt(Random.Range(0, islands[isl].grassTotalWeight));
			}
		}
		Vector3 pos = transform.TransformPoint(new Vector3(Random.insideUnitCircle.x * islands[isl].bounds, 300f, Random.insideUnitCircle.y * islands[isl].bounds));
		RaycastHit hit;
		if (Physics.Raycast(pos, Vector3.down, out hit, Mathf.Infinity, -1))
		{
			if (hit.collider.gameObject == islands[isl].island)
			{
				GameObject obj = Instantiate(islands[isl].grasstypeSpawns[spawnIndex].spawn, hit.point, Quaternion.LookRotation(hit.normal)) as GameObject;
				obj.transform.Rotate(Vector3.forward * Random.Range(0f, 360f));
			}
		}
	}

	void SpawnSmallIsland(int isl)
	{
		GameObject islandObj = Instantiate(islands[isl].smallIslandPrefab, transform.TransformPoint(islands[isl].smallIslandSpawnLocations[Random.Range(0, islands[isl].smallIslandSpawnLocations.Length)]), islands[isl].smallIslandPrefab.transform.rotation) as GameObject;
		Vector3 pos = new Vector3(Random.Range(-1f, 1f), 300f, Random.Range(-1f, 1f)) + islandObj.transform.position;
		RaycastHit hit;
		if (islands[isl].useIslandCrates)
		{
			if (Physics.Raycast(pos, Vector3.down, out hit, Mathf.Infinity, -1))
			{
				GameObject crateObj = Instantiate(islands[isl].cratePrefab, hit.point + Vector3.up * 0.5f, Quaternion.LookRotation(hit.normal)) as GameObject;
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