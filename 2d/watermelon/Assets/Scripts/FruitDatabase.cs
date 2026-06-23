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
            new FruitDefinition { name = "Cherry",       color = new Color(0.842f, 0.151f, 0.070f),  radius = 0.26f, mergeScore = 2 },
            new FruitDefinition { name = "Strawberry",   color = new Color(0.876f, 0.208f, 0.121f),  radius = 0.30f, mergeScore = 4 },
            new FruitDefinition { name = "Grape",        color = new Color(0.494f, 0.389f, 0.652f),  radius = 0.34f, mergeScore = 6 },
            new FruitDefinition { name = "Orange",       color = new Color(0.963f, 0.541f, 0.011f),  radius = 0.40f, mergeScore = 8 },
            new FruitDefinition { name = "Apple",        color = new Color(0.894f, 0.147f, 0.115f),  radius = 0.46f, mergeScore = 10 },
            new FruitDefinition { name = "Pear",         color = new Color(0.841f, 0.791f, 0.171f),  radius = 0.50f, mergeScore = 12 },
            new FruitDefinition { name = "Peach",        color = new Color(0.993f, 0.690f, 0.663f),  radius = 0.54f, mergeScore = 14 },
            new FruitDefinition { name = "Pineapple",    color = new Color(0.993f, 0.855f, 0.192f),  radius = 0.60f, mergeScore = 16 },
            new FruitDefinition { name = "Melon",        color = new Color(0.797f, 0.851f, 0.475f),  radius = 0.68f, mergeScore = 18 },
            new FruitDefinition { name = "Watermelon",   color = new Color(0.317f, 0.544f, 0.179f),  radius = 0.78f, mergeScore = 20 },
        };
    }
}
