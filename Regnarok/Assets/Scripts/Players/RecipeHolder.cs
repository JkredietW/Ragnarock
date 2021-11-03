using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeHolder : MonoBehaviour
{
    public CraftingRecipe recipe;

    [SerializeField] TextMeshProUGUI resourceAmount1, resourceAmount2;
    [SerializeField] Image resultSprite, resourceSprite1, resourceSprite2;

    //color
    [HideInInspector] public Color normalColor = Color.white;
    [HideInInspector] public Color disabledColor = new Color(1, 1, 1, 0);

    public void UpdateUi()
    {
        resultSprite.sprite = recipe.resultSprite;

        resourceAmount1.text = recipe.resourceAmount1.ToString();
        resourceSprite1.sprite = recipe.resourceSprite1;

        if (recipe.craft.itemsNeeded.Count > 1)
        {
            resourceAmount2.text = recipe.resourceAmount2.ToString();
            resourceSprite2.sprite = recipe.resourceSprite2;
            resourceSprite2.color = normalColor;
        }
        else
        {
            resourceAmount2.text = "";
            resourceSprite2.sprite = recipe.resourceSprite2;
            resourceSprite2.color = disabledColor;
        }
    }
}
