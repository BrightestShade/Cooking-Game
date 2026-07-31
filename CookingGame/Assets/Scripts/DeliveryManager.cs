using UnityEngine;
using System.Collections.Generic;

public class DeliveryManager : MonoBehaviour
{
    public static DeliveryManager Instance {get; private set;}
    [SerializeField] private RecipeListSO recipeListSO;


    private List<RecipeSO> waitingRecipeSOList;

    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipesMax = 4;

    private void Awake()
    {
        Instance = this;


        waitingRecipeSOList = new List<RecipeSO>();
    }

    private void Update()
    {
        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if(waitingRecipeSOList.Count < waitingRecipesMax)
            {
                RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[Random.Range(0, recipeListSO.recipeSOList.Count)];
                    Debug.Log(waitingRecipeSO.recipeName);
                waitingRecipeSOList.Add(waitingRecipeSO);
            }
            
        }
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < waitingRecipeSOList.Count; i++)
        {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            if (waitingRecipeSO.kitchenObjectsSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
            {
                // has the same number of ingredients
                bool plateContentsMatchesRecipe = true;
                foreach (KitchenObjectsSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectsSOList)
                {
                    // cycling through all ingredients in the recipe
                    bool ingredientFound = false;
                    foreach (KitchenObjectsSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
                    {
                        // cycling through all ingredients on the plate
                        if (plateKitchenObjectSO == recipeKitchenObjectSO)
                        {
                            // Ingredients match
                            ingredientFound = true;
                            break;
                        }
                    }
                    if (!ingredientFound)
                    {
                        // This recipe ingredient was not found on the plate
                        plateContentsMatchesRecipe = false;
                    }
                }
                if (plateContentsMatchesRecipe)
                {
                    // Player delivered the correct recipe
                    waitingRecipeSOList.RemoveAt(i);
                    Debug.Log("Player delivered the correct recipe");
                        return;
                }
            }
            //no matches found
            //Player did not deliver a correct recipe
            Debug.Log("Player did not deliver the correct recipe");
        }
    }
}
