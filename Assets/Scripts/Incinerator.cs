using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Incinerator : MonoBehaviour {

	[SerializeField] TellParent tellParent;

	[SerializeField] ParticleSystem smokeParticles;
	[SerializeField] ParticleSystem fireParticles;

	AchievementManager achievementManager;

	void Start() {
		achievementManager = FindObjectOfType<AchievementManager>();
	}

	void Update() {
		if(tellParent.currentColliders.Count > 0) {
			foreach(Collider col in tellParent.currentColliders) {
				if(col && col.CompareTag("Item")) {
					ItemHandler itemHandler = col.GetComponent<ItemHandler>();
					if(itemHandler) {
						IncinerateObject(itemHandler.gameObject);
					}
				}
			}
		}
	}

	public void IncinerateObject (GameObject incin)
    {
		Destroy(incin.gameObject);
		smokeParticles.Play();
		fireParticles.Play();
		achievementManager.GetAchievement(12); // Destruction achievement
	}

	public void IncinerateEffects ()
    {
		smokeParticles.Play();
		fireParticles.Play();
		achievementManager.GetAchievement(12); // Destruction achievement
	}
}