using UnityEngine;

[CreateAssetMenu(fileName = "CookingChoice", menuName = "ScriptableObjects/CookingChoice")]
public class CookingData : ScriptableObject
{
    public enum orderCategory {Food, Drink, Speciality, Dessert }

    [Header("ingredient")]
    new public string name;
    public CookingData[] nextChoices;
    public Sprite image;
    public CookingData oppositeIngredient;
    public orderCategory category;

    public bool isFinalIngredient() => nextChoices.Length == 0;
}


