using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TellParent : MonoBehaviour {

	public List<Collider> currentColliders = new List<Collider>();
	[SerializeField] private Component parent;
	[SerializeField] private string message;

	void OnTriggerEnter(Collider col) {
		if(!currentColliders.Contains(col)) {
			currentColliders.Add(col);
		}
		SendFallDamage(col);
		if (parent != null)
		{
			ITakeInput output = parent.GetComponent<ITakeInput>();
			if (output != null)
			{
				output.TakeInput(message);
			}
		}
	}

	void OnTriggerStay(Collider col) {
		if(!currentColliders.Contains(col)) {
			currentColliders.Add(col);
		}
	}

	void OnTriggerExit(Collider col) {
		if(currentColliders.Contains(col)) {
			currentColliders.Remove(col);
		}
	}

	void SendFallDamage(Collider col)
    {
		if (parent)
		{
			if (parent.GetComponent<ITakeFallDamage>() != null)
			{
				parent.GetComponent<ITakeFallDamage>().TakeFallDamage(col.gameObject);

			}
		}
    }

	public Collider[] Colliders ()
    {
		List<Collider> tempCols = new List<Collider>();
		foreach (Collider col in currentColliders)
        {
			if (col != null)
            {
				tempCols.Add(col);
            }
        }
		currentColliders = tempCols;
		return currentColliders.ToArray();
    }
}
