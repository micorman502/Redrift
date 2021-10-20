using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeResource : MonoBehaviour {

	public GameObject applePrefab;
	public Transform[] appleSpawnLocations;

	public float appleSpawnChance = 0.8f;

	public List<GameObject> apples = new List<GameObject>();

	[SerializeField] float growthSpeed;
	[SerializeField] float instantGrowthChance; //mainly for when you spawn in - so you don't have to wait a while for trees to grow in
	[SerializeField] float growthSpeedRandomisation;
	[SerializeField] float maxSize;
	float growthPercent;
	bool applesSpawned = false;
	ResourceHandler resource;


	[HideInInspector] public bool spawnApples = true;

	void Start() {
		resource = gameObject.GetComponent<ResourceHandler>();
		growthSpeed = growthSpeed + Random.Range(0, growthSpeedRandomisation);
		resource.enabled = false;
		if (Random.Range(0f, 1f) <= instantGrowthChance)
		{
			growthPercent = 99f;
		}
		else
		{
			transform.localScale = new Vector3((growthPercent / 100) * maxSize, (growthPercent / 100) * maxSize, (growthPercent / 100) * maxSize);
		}
	}

    void Update()
    {
		if (growthPercent < 100)
		{
			growthPercent += growthSpeed * Time.deltaTime;
			if (growthPercent > 100)
			{
				growthPercent = 100;
				SpawnApples();
				resource.enabled = true;
			}
			transform.localScale = new Vector3((growthPercent / 100) * maxSize, (growthPercent / 100) * maxSize, (growthPercent / 100) * maxSize);
		}
    }

	void SpawnApples ()
    {
		if (spawnApples)
		{
			if (!applesSpawned)
			{
				foreach (Transform spawn in appleSpawnLocations)
				{
					if (Random.Range(0f, 1f) < appleSpawnChance)
					{
						GameObject appleObj = Instantiate(applePrefab, spawn.position, spawn.rotation) as GameObject;
						appleObj.GetComponent<Apple>().Attach(gameObject.transform.position);
						Rigidbody appleRB = appleObj.GetComponent<Rigidbody>();
						apples.Add(appleObj);
						if (appleRB)
						{
							Destroy(appleRB);
						}
					}
				}
				applesSpawned = true;
			}
		}
	}

    public void DropFruits() {
		foreach(GameObject apple in apples) {
			if(apple) {
				Instantiate(applePrefab, apple.transform.position, apple.transform.rotation);
				Destroy(apple);
			}
		}
	}

	public float GetGrowthState ()
    {
		return growthPercent;
    }

	public void SetGrowthState (float state)
    {
		growthPercent = state;
		if (growthPercent > 100)
        {
			resource.enabled = true;
		}
    }
}
