using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Incinerator : MonoBehaviour, IGetTriggerInfo {

	[SerializeField] TellParent tellParent;

	[SerializeField] ParticleSystem smokeParticles;
	[SerializeField] ParticleSystem fireParticles;


	public void GetTriggerInfo (Collider col)
    {
		DestroyObject(col);
	}

	public void GetTriggerInfoRepeating (Collider col)
    {
		DestroyObject(col);
	}

	void DestroyObject (Collider col)
    {
		if (col.CompareTag("Item"))
		{
			DestroyObject(col.gameObject);
		}
	}

	void DestroyObject (GameObject obj)
    {
		ItemHandler itemHandler = obj.GetComponent<ItemHandler>();
		if (itemHandler)
		{
			Destroy(itemHandler.gameObject);
			smokeParticles.Play();
			fireParticles.Play();
			AchievementManager.Instance.GetAchievement(12); // Destruction achievement
		}
	}
}