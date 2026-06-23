using UnityEngine;

public static class FruitSpriteLoader
{
    public static Sprite Load(string fruitName)
    {
        if (string.IsNullOrEmpty(fruitName))
        {
            return null;
        }

        string key = "Fruits/" + fruitName.ToLowerInvariant();
        return Resources.Load<Sprite>(key);
    }
}
