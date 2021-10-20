using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Recipe : ScriptableObject {
	public WorldItem[] inputs;
	public WorldItem[] replacedItems;
	public WorldItem output;
	public RecipeManager.Categories[] categories;

	public bool UsesItem (WorldItem item)
    {
		foreach (WorldItem input in inputs)
        {
			if (item.item == input.item)
            {
				return true;
            }
        }
		return false;
    }

	public WorldItem[] TotalOutputs () //return outputs, including replacedItems
    {
		List<WorldItem> newOutputs = new List<WorldItem>();

		for (int i = 0; i < replacedItems.Length; i++)
		{
			newOutputs.Add(replacedItems[i]);
		}
		newOutputs.Add(output);

		return newOutputs.ToArray();
    }
}
