using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallIsland : MonoBehaviour, IItemSaveable {

	public int saveID;

	public Vector3 moveAmount;

	float speed = 0.5f;
	[SerializeField] float minSpeed, maxSpeed;
	bool loaded;

	void Start ()
    {
		if (!loaded)
        {
			speed = Random.Range(minSpeed, maxSpeed);
        }
		if (speed == 0)
        {
			Destroy(gameObject);
        }
    }

	void FixedUpdate ()
    {
		transform.position += moveAmount * Time.fixedDeltaTime * speed;
		if (transform.localPosition.z >= 200f)
		{
			if (transform.childCount > 0)
			{
				Transform[] children = transform.GetComponentsInChildren<Transform>();

				foreach (Transform child in children)
				{
					if (!child.CompareTag("Player"))
					{
						Destroy(child.gameObject);
					}
				}
			}
			Destroy(gameObject);
		}
	}

	public void GetData(out ItemSaveData data, out ObjectSaveData objData, out bool dontSave)
	{
		ItemSaveData newData = new ItemSaveData();
		ObjectSaveData newObjData = new ObjectSaveData(transform.position, transform.rotation, saveID);

		newData.floatVal = speed;

		data = newData;
		objData = newObjData;
		dontSave = false;
	}

	public void SetData(ItemSaveData data, ObjectSaveData objData)
	{
		transform.position = objData.position;
		transform.rotation = objData.rotation;

		speed = data.floatVal;

		loaded = true;
	}
}