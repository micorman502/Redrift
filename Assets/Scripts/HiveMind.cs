using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiveMind : MonoBehaviour {
	public static HiveMind Instance;
	public List<ResourceHandler> worldResources = new List<ResourceHandler>();

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    public void AddResource(ResourceHandler handler) {
		worldResources.Add(handler);
	}

	public void RemoveResource(ResourceHandler handler) {
		worldResources.Remove(handler);
	}
}
