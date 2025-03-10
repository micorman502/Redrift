﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightItem : MonoBehaviour, IItemSaveable {

	[SerializeField] int saveID;
	public float[] intensities = { 0f, 0.25f, 0.5f, 1f };
	public int intensityNum = 0;

	public bool active = true;

	new Light light;

	void Awake() {
		light = GetComponentInChildren<Light>();
	}

	void Start() {
		UpdateActive();
		UpdateIntensity();
	}

	public void SetIntensity(int newIntensityNum) {
		intensityNum = newIntensityNum;
		if(intensities[intensityNum] == 0) {
			active = false;
			UpdateActive();
		} else if(!active) {
			active = true;
			UpdateActive();
		}
		UpdateIntensity();
	}

	public void IncreaseIntensity() {
		intensityNum++;
		if(intensityNum >= intensities.Length) {
			intensityNum = 0;
		}
		if(intensities[intensityNum] == 0) {
			active = false;
			UpdateActive();
		} else if(!active) {
			active = true;
			UpdateActive();
		}
		UpdateIntensity();
	}

	void UpdateActive() {
		light.enabled = active;
	}

	void UpdateIntensity() {
		light.intensity = intensities[intensityNum];
	}

	public void GetData(out ItemSaveData data, out ObjectSaveData objData, out bool dontSave)
	{
		ItemSaveData newData = new ItemSaveData();
		ObjectSaveData newObjData = new ObjectSaveData(transform.position, transform.rotation, saveID);

		newData.num = intensityNum;

		data = newData;
		objData = newObjData;
		dontSave = false;
	}

	public void SetData(ItemSaveData data, ObjectSaveData objData)
	{
		transform.position = objData.position;
		transform.rotation = objData.rotation;

		SetIntensity(data.num);
	}
}
