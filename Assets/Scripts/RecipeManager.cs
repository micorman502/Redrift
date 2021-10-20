using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class RecipeManager : MonoBehaviour {

	public enum Categories { All, Construction, Resources, Tools, Automation, Decoration };

	[SerializeField] Sprite[] categoryIcons;
	[SerializeField] GameObject categoriesContainer;
	[SerializeField] GameObject categoryButtonPrefab;

	public Transform craftingRecipeContainer;
	public RectTransform craftingScrollRectTransform;
	ScrollRect craftingScrollRect;

	public Recipe[] recipes;

	public GameObject craftingRecipePrefab;
	List<CraftingRecipe> craftingRecipes = new List<CraftingRecipe>();

	bool createImmediateFreeRecipes;

	void Awake() {
		craftingScrollRect = craftingScrollRectTransform.GetComponent<ScrollRect>();
	}

	void Start() {
		InitializeCategories();
		InitializeRecipes(Categories.All);
	}

	public void InitializeRecipes(Categories c) {
		ClearRecipes();
		int i = 0;
		Inventory tempInv = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
		foreach (Recipe recipe in recipes) {
			if(recipe.categories.Contains(c) || c == 0) {
				GameObject recipeObj = Instantiate(craftingRecipePrefab, craftingRecipeContainer);
				recipeObj.GetComponent<RecipeListItem>().Setup(recipe, tempInv);

				RectTransform rectTransform = recipeObj.GetComponent<RectTransform>();
				Vector3 tempSize = rectTransform.sizeDelta;
				tempSize.y = Mathf.Ceil(recipe.inputs.Length / 3f) * 100f;
				rectTransform.sizeDelta = tempSize;
				craftingScrollRect.verticalScrollbar.value = 1f;
				if (createImmediateFreeRecipes)
				{
					CraftingRecipe cr = recipeObj.GetComponent<CraftingRecipe>();
					cr.SetFreeRecipe(true);
					craftingRecipes.Add(cr);
				}
				else
				{
					craftingRecipes.Add(recipeObj.GetComponent<CraftingRecipe>());
				}
			}
			i++;
		}
	}

	void ClearRecipes() {
		foreach(Transform recipeTrans in craftingRecipeContainer) {
			if(recipeTrans.GetComponent<RecipeListItem>()) {
				Destroy(recipeTrans.gameObject);
			}
		}
	}

	void InitializeCategories() {
		int numCategories = System.Enum.GetNames(typeof(Categories)).Length;
		string[] categoryNames = System.Enum.GetNames(typeof(Categories));
		for(int i = 0; i <= numCategories - 1; i++) {
			int recipeToInitialize = i;
			GameObject obj = Instantiate(categoryButtonPrefab, categoriesContainer.transform);
			obj.GetComponent<Button>().onClick.AddListener(delegate { InitializeRecipes((Categories)recipeToInitialize); });
			obj.GetComponentInChildren<Text>().text = categoryNames[i];
			obj.GetComponentsInChildren<Image>()[1].sprite = categoryIcons[i];
		}
	}

	public void RemoveAllCraftingCosts () //called from inventory loadcreativemode
    {
		if (craftingRecipes.Count > 0)
		{
			foreach (CraftingRecipe craftingRecipe in craftingRecipes)
			{
				craftingRecipe.SetFreeRecipe(true);
			}
		} else
        {
			createImmediateFreeRecipes = true;
        }
    }
}
