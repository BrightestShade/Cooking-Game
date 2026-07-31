using UnityEngine;
using System;
using System.Collections.Generic;

// [CreateAssetMenu()] // this comment is so nobody can create antoher recipe list SO
public class RecipeListSO : ScriptableObject
{
    public List<RecipeSO> recipeSOList;
}
