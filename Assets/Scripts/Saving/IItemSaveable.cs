using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItemSaveable
{
    void GetData(out ItemSaveData data, out ObjectSaveData objData, out bool dontSave);

    void SetData(ItemSaveData data, ObjectSaveData objData);
	/* GENERIC
    public int saveID;
    	public void GetData (out ItemSaveData data,out ObjectSaveData objData, out bool dontSave)
    {
		ItemSaveData newData = new ItemSaveData();
		ObjectSaveData newObjData = new ObjectSaveData(transform.position, transform.rotation, saveID);


		data = newData;
		objData = newObjData;
		dontSave = false;
    }

	public void SetData (ItemSaveData data, ObjectSaveData objData)
    {
		transform.position = objData.position;
		transform.rotation = objData.rotation;
	}
    */
}
