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
}
