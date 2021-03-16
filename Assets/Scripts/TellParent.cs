using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TellParent : MonoBehaviour {

	public List<Collider> currentColliders = new List<Collider>();
	[SerializeField] private Component parent;

	void OnTriggerEnter(Collider col) {
		if(!currentColliders.Contains(col)) {
			currentColliders.Add(col);
		}
		SendFallDamage(col);
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
}
