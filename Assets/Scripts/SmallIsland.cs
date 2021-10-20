using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallIsland : MonoBehaviour {

	public enum IslandType {Light, Dark};
	public IslandType islandType;
	public Vector3 moveAmount;
	public int requiredObjects; //amount of objects required for this to move
	private float fakeGravity;

	public float speed = 0.5f;
	
	void Update () {
		transform.position += moveAmount * Time.deltaTime * speed;
	}

    void FixedUpdate()
    {
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
		if (transform.childCount < requiredObjects)
        {
			fakeGravity += 0.001f;
			transform.position += Vector3.down * fakeGravity;
        } else
        {
			fakeGravity = fakeGravity / 1.05f;
			if (fakeGravity <= 0.005)
            {
				fakeGravity = 0;
            }
        }
		if (transform.position.y <= -500f)
        {
			Destroy(gameObject);
        }
    }
}