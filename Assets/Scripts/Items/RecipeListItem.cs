using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeListItem : MonoBehaviour
{
    [SerializeField] Image baseImage;
    [SerializeField] Transform craftingSlotHolder;
    [SerializeField] GameObject craftingSlotPrefab;
    [SerializeField] CraftingSlot outputSlot;
    [SerializeField] Text outputAmount;
    [SerializeField] Color craftableTint;
    [SerializeField] Color uncraftableTint;
    Inventory targetInventory;
    Recipe recipe;
    
    public void Setup (Recipe _recipe, Inventory _inventory)
    {
        targetInventory = _inventory;
        recipe = _recipe;

        if (recipe.output.amount > 1)
        {
            outputAmount.enabled = true;
            outputAmount.text = recipe.output.amount.ToString();
        }
        else
        {
            outputAmount.enabled = false;
        }

        foreach (WorldItem item in recipe.inputs)
        {
            GameObject craftingSlotObj = Instantiate(craftingSlotPrefab, craftingSlotHolder);
            CraftingSlot craftingSlot = craftingSlotObj.GetComponent<CraftingSlot>();
            craftingSlot.Setup(item);
        }

        outputSlot.Setup(recipe.output);
    }

    public void InventoryUpdate()
    {
        if (!recipe)
            return;
        if (targetInventory.CheckRecipe(recipe)) //idk how else to check
        {
            baseImage.color = craftableTint;
        }
        else
        {
            baseImage.color = uncraftableTint;
        }
    }

    public void OnItemPointerEnter()
    {
        InventoryEvents.SetHoveredItem(recipe.output.item, null);
    }

    public void OnItemPointerExit()
    {
        InventoryEvents.LeaveHoveredItem();
    }

    public void OnRecipeClick()
    {
        InventoryEvents.ConstructItem(recipe);
    }
}
