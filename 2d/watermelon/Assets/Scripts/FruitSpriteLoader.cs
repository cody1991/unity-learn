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

    public static Sprite LoadForTier(int tier, string fruitName)
    {
        if (UserFruitSkinStorage.TryLoadSprite(tier, out Sprite userSprite))
        {
            return userSprite;
        }

        return Load(fruitName);
    }
}
