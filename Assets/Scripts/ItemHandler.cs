using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour {

	public Item item;
	public void GetDestroyed ()
    {
        GridPosition gr = gameObject.GetComponent<GridPosition>(); //not very clean but ehhhh
        ElevatorBlock eb = gameObject.GetComponent<ElevatorBlock>();
        if (gr)
        {
            gr.GetDestroyed();
        }
        if (eb)
        {
            eb.GetDestroyed();
        }
            Destroy(gameObject);
    }
}