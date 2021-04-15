using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AutoMiner : MonoBehaviour
{

	NavMeshAgent agent;

	HiveMind hive;

	public GameObject toolHolder;

	public ResourceHandler target;
	[SerializeField] AutoMinerDock dockingStation;
	[SerializeField] GameObject internalTarget; //what the destination is actually set to

	float interactRange = 2.5f;

	Animator animator;

	[SerializeField] bool moving = false;
	[SerializeField] bool gathering = false;

	public List<Item> items = new List<Item>();
	public List<int> itemAmounts = new List<int>();
	[SerializeField] int maxItemThreshold;
	int totalItems;

	float gatherTime = 0f;

	public Item currentToolItem = null;
	GameObject currentToolObj = null;

	PlayerController player;
	[SerializeField] bool full;

	void Start()
	{
		full = false;
		hive = GameObject.FindGameObjectWithTag("ScriptHolder").GetComponent<HiveMind>();
		player = FindObjectOfType<PlayerController>();
		animator = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();
		GetComponent<AudioSource>().outputAudioMixerGroup = FindObjectOfType<SettingsManager>().audioMixer.FindMatchingGroups("Master")[0];
	}

	void Update()
	{
		if (agent.isOnNavMesh) {
			if (currentToolItem) {
				if (full)
				{
					SetGathering(false);
					SetMoving(true);
					if (dockingStation)
					{
						SetDestination(dockingStation.gameObject);
						if (Vector3.Distance(transform.position, dockingStation.transform.position) <= interactRange)
						{
							dockingStation.UnloadMiner(this, out bool accepted);
						}
					}
				} else if (target) {
					if (dockingStation && internalTarget == dockingStation.gameObject) //if still trying to go to a dock while not full, find another target
                    {
						SetDestination(target.gameObject);
                    }
					if (!moving)
					{
						SetMoving(true);
					}
					if (!full)
					{
						if (Vector3.Distance(transform.position, target.transform.position) <= interactRange) //if close to a resource
						{
							SetGathering(true);
							SetMoving(false);
							gatherTime += Time.deltaTime * currentToolItem.speed;
							if (gatherTime >= target.resource.gatherTime)
							{
								int gathered = target.ToolGather(currentToolItem, out Item item);
								AddItem(item, gathered);
								CheckItems();
								gatherTime = 0;
							}
						}
					}
					else
					{
						if (dockingStation) 
						{
							if (Vector3.Distance(transform.position, dockingStation.transform.position) <= interactRange) // if close to a docking station
							{
								dockingStation.UnloadMiner(this, out bool accepted);
							}
						}
					}
				} //if no target
				else
				{
					SetGathering(false);
					gatherTime = 0f;
					FindNearestTarget();
				}
			}
		}
	}

	public void FindNearestTarget()
	{
		ResourceHandler closestHandler = null;
		float closestDistance = Mathf.Infinity;
		foreach (ResourceHandler resourceHandler in hive.worldResources)
		{
			if (resourceHandler)
			{
				float dist = Vector3.Distance(transform.position, resourceHandler.transform.position);
				if (dist < closestDistance && currentToolItem.toolToughness >= resourceHandler.resource.toughness && resourceHandler.enabled)
				{
					closestDistance = dist;
					closestHandler = resourceHandler;
				}
			}
		}
		if (closestHandler != null)
		{
			target = closestHandler;
			if (agent.isStopped)
			{
				agent.isStopped = false;
			}
			//target = hive.worldResources[Random.Range(0, hive.worldResources.Count)];
			SetDestination(target.gameObject);
			if (Vector3.Distance(agent.destination, target.transform.position) > interactRange)
			{
				target = null;
			}
		}
		else
		{
			if (!agent.isStopped)
			{
				agent.isStopped = true;
				SetMoving(false);
			}
		}
	}

	public void CheckItems() 
	{
		totalItems = 0;
		foreach (int amt in itemAmounts)
		{
			totalItems += amt;
		}
		if (totalItems >= maxItemThreshold)
        {
			full = true;
        } else {
			full = false;
        }
		if (full)
		{
			if (dockingStation == null)
			{
				dockingStation = GameObject.FindObjectOfType<AutoMinerDock>();
				if (dockingStation == null || Vector3.Distance(transform.position, dockingStation.transform.position) > 100f)
				{
					agent.isStopped = true;
				}
				else if (Vector3.Distance(transform.position, dockingStation.transform.position) < 100f)
				{
					SetDestination(dockingStation.gameObject);
					SetGathering(false);
				}
			}
			else if (Vector3.Distance(transform.position, dockingStation.transform.position) < 100f)
			{
				SetDestination(dockingStation.gameObject);
				SetGathering(false);
			}
		}
	}

	void SetDestination (GameObject obj) //partially for debugging and making the actual target more obvious
    {
		internalTarget = obj;
		agent.SetDestination(obj.transform.position);
	}

	public void DumpItem(int itemNum)
	{
		if (items.Count > itemNum)
		{
			RemoveItems(0, 1);
			Instantiate(items[itemNum].prefab, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
		}
	}

	public void SetTool(Item item)
	{

		if (currentToolObj)
		{
			Destroy(currentToolObj);
		}

		if (currentToolItem)
		{
			player.inventory.AddItem(currentToolItem, 1);
		}

		currentToolItem = item;
		GameObject obj = Instantiate(item.prefab, toolHolder.transform) as GameObject;

		Rigidbody objRB = obj.GetComponent<Rigidbody>();
		if (objRB)
		{
			objRB.isKinematic = true;
		}
		obj.tag = "Untagged";
		foreach (Transform trans in obj.transform)
		{
			trans.tag = "Untagged";
		}

		currentToolObj = obj;
	}

	public Item GatherTool()
	{
		Item returnItem = currentToolItem;
		currentToolItem = null;
		Destroy(currentToolObj);
		return returnItem;
	}

	public void ClearItems()
	{

		items.Clear();
		itemAmounts.Clear();
		CheckItems();
	}

	void AddItem(Item item, int amount)
	{
		if (currentToolItem.toolToughness >= target.resource.toughness)
		{
			bool hasItem = false;

			int i = 0;
			foreach (Item _item in items)
			{
				if (_item.id == item.id)
				{
					hasItem = true;
					itemAmounts[i] += amount;
				}
				i++;
			}

			if (!hasItem)
			{
				items.Add(item);
				itemAmounts.Add(amount);
			}
		}
		else
		{ //if you cannot gather this resource, retry and find the nearest mineable resource.
			FindNearestTarget();
		}
	}

	public void RemoveItems(int item, int amount)
	{
		itemAmounts[item] -= amount;
		if (itemAmounts[item] <= 0)
		{
			itemAmounts.RemoveAt(item);
			items.RemoveAt(item);
		}
	}

	public void TakeItems(int item, int amount, out int takenSuccesfully)
	{
		int taken = 0;
		int endAmt = itemAmounts[item] - amount;
		if (endAmt <= 0)
		{
			taken = itemAmounts[item];
			itemAmounts.RemoveAt(item);
			items.RemoveAt(item);
		}
		else
		{
			taken = amount;
			itemAmounts[item] -= amount;
		}
		takenSuccesfully = taken;
	}

	public void TakeItemStack(int item, out int taken, out Item itempassed)
	{
		if (items.Count > item)
		{
			itempassed = items[item];
			int takenAmt = 0;
			if (items.Count > 0)
			{
				TakeItems(item, itemAmounts[item], out takenAmt);
			}
			taken = takenAmt;
		}
		else
		{
			itempassed = null;
			taken = 0;
		}

	}

	public int GetStackSize(int item)
	{
		if (itemAmounts.Count > item)
		{
			return itemAmounts[item];
		}
		else
		{
			return 0;
		}
	}

	public List<Item> GetItems ()
    {
		return items;
    }

	void SetMoving(bool state)
	{
		moving = state;
		animator.SetBool("Moving", moving);
	}

	void SetGathering(bool state)
	{
		gathering = state;
		animator.SetBool("Gathering", gathering);
	}
}
