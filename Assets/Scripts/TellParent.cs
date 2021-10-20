using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TellParent : MonoBehaviour {
	[SerializeField] bool sendInitialMessage;
	[SerializeField] bool sendRepeatingMessage;
	[SerializeField] GameObject target;
	IGetTriggerInfo realTarget;

    private void Awake()
    {
		realTarget = target.GetComponent<IGetTriggerInfo>();
    }
    void OnTriggerEnter(Collider col) {
		if (sendInitialMessage)
			realTarget.GetTriggerInfo(col);
	}

	void OnTriggerStay(Collider col) {
		if (sendRepeatingMessage)
			realTarget.GetTriggerInfoRepeating(col);
	}
}
