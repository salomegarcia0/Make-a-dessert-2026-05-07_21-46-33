using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu()]
public class RecipeSO: ScriptableObject
{

	public List<IngredientObject> IngredientObjectList;
	public string RecipeName;

}
