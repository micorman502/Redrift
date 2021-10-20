using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour, IItemSaveable, ITooltip {

	[SerializeField] string tooltip = "Hold [E] to pickup";
	public Item item;

	[SerializeField] int saveID;
	[SerializeField] bool dontSave;
	public void GetData(out ItemSaveData data, out ObjectSaveData objData, out bool _dontSave)
	{
		ItemSaveData newData = new ItemSaveData();
		ObjectSaveData newObjData = new ObjectSaveData(transform.position, transform.rotation, saveID);


		data = newData;
		objData = newObjData;
		_dontSave = dontSave;
	}

	public string GetTooltip ()
    {
		return tooltip;
    }

	public void SetTooltip (string _tooltip)
    {
		tooltip = _tooltip;
    }

	public void SetData(ItemSaveData data, ObjectSaveData objData)
	{
		transform.position = objData.position;
		transform.rotation = objData.rotation;
	}

	public void SetSaveID (int newID)
    {
		saveID = newID;
    }
}