using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceHandler : MonoBehaviour {

	public Resource resource;

	public int health;

	HiveMind hive;

	void Start() {
		if(health == 0) {
			health = resource.maxGathers;
		}
		hive = GameObject.FindGameObjectWithTag("ScriptHolder").GetComponent<HiveMind>();
	}

	public int Gather(int amount) { //int return is how many items were succesfully collected.
		if (!resource.infiniteGathers)
		{
			int startingHealth = health;
			health -= amount;
			if (health <= 0 && !resource.infiniteGathers)
			{
				hive.RemoveResource(this);
				if (resource.type == Resource.ResourceType.Tree)
				{
					GetComponent<TreeResource>().DropFruits();
				}
				Destroy(gameObject);
			}
			if (startingHealth - amount >= 0)
			{
				return amount;
			}
			else
			{
				return startingHealth;
			}
		} else //if you have infinite gathers, just return the amount tryingto be gathered.
        {
			return amount;
        }
	}

	public int ToolGather (Item tool, out Item itemReturn) //gather according to an item's stats
    {
		itemReturn = null;
		for (int i = 0; i < resource.resourceItems.Length; i++)
		{
			if (Random.Range(0f, 1f) <= resource.chances[i])
			{
				itemReturn = resource.resourceItems[i]; //Due to how this works, it matters how resource items are arranged! The later ones have higher priority.
			}
		}
		int gatherAmt = 1;
		if (tool)
		{
			if (tool.toolToughness >= resource.toughness)
			{
				if (!resource.infiniteGathers)
				{
					if (tool.gatherAmount > 1)
					{
						gatherAmt = tool.gatherAmount;
					}
					int startingHealth = health;
					health -= gatherAmt;
					if (health <= 0 && !resource.infiniteGathers)
					{
						hive.RemoveResource(this);
						if (resource.type == Resource.ResourceType.Tree)
						{
							GetComponent<TreeResource>().DropFruits();
						}
						Destroy(gameObject);
					}
					if (startingHealth - gatherAmt >= 0)
					{
						return gatherAmt;
					}
					else
					{
						return startingHealth;
					}
				}
				else //if you have infinite gathers, just return the amount tryingto be gathered.
				{
					return gatherAmt;
				}
			}
			else
			{ //if tool is too weak... 
				return 0;
			}
		} else
        { //if you are using your hand, gather only one item
			return Gather(1);
        }
	}
}
