using UnityEngine;

[CreateAssetMenu(fileName = "FruitDatabase", menuName = "Watermelon/Fruit Database")]
public class FruitDatabase : ScriptableObject
{
    public FruitDefinition[] fruits;

    public int MaxTier => fruits.Length - 1;

    public FruitDefinition Get(int tier)
    {
        tier = Mathf.Clamp(tier, 0, MaxTier);
        return fruits[tier];
    }

    public static FruitDefinition[] DefaultFruits()
    {
        return new[]
        {
            new FruitDefinition { name = "Cherry",       color = new Color(0.85f, 0.1f, 0.2f),   radius = 0.26f, mergeScore = 2 },
            new FruitDefinition { name = "Strawberry",   color = new Color(0.95f, 0.2f, 0.25f),  radius = 0.30f, mergeScore = 4 },
            new FruitDefinition { name = "Grape",        color = new Color(0.55f, 0.2f, 0.75f),  radius = 0.34f, mergeScore = 6 },
            new FruitDefinition { name = "Orange",       color = new Color(1f, 0.45f, 0.05f),    radius = 0.40f, mergeScore = 8 },
            new FruitDefinition { name = "Apple",        color = new Color(0.9f, 0.15f, 0.15f),  radius = 0.46f, mergeScore = 10 },
            new FruitDefinition { name = "Pear",         color = new Color(0.75f, 0.85f, 0.2f),  radius = 0.50f, mergeScore = 12 },
            new FruitDefinition { name = "Peach",        color = new Color(1f, 0.65f, 0.55f),    radius = 0.54f, mergeScore = 14 },
            new FruitDefinition { name = "Pineapple",    color = new Color(0.95f, 0.8f, 0.1f),   radius = 0.60f, mergeScore = 16 },
            new FruitDefinition { name = "Melon",        color = new Color(0.45f, 0.8f, 0.35f),  radius = 0.68f, mergeScore = 18 },
            new FruitDefinition { name = "Watermelon",   color = new Color(0.15f, 0.55f, 0.2f),  radius = 0.78f, mergeScore = 20 },
        };
    }
}
