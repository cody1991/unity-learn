using System.IO;
using UnityEngine;

public static class UserFruitSkinStorage
{
    const float PixelsPerUnit = 100f;

    public static void Save(int tier, byte[] imageBytes)
    {
        if (imageBytes == null || imageBytes.Length == 0)
        {
            return;
        }

        Directory.CreateDirectory(SkinDirectory);
        File.WriteAllBytes(GetPath(tier), imageBytes);
    }

    public static bool TryLoadSprite(int tier, out Sprite sprite)
    {
        sprite = null;
        string path = GetPath(tier);
        if (!File.Exists(path))
        {
            return false;
        }

        byte[] bytes = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!texture.LoadImage(bytes))
        {
            Object.Destroy(texture);
            return false;
        }

        texture.name = "UserFruitSkin_" + tier;
        sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), PixelsPerUnit);
        return true;
    }

    public static void Delete(int tier)
    {
        string path = GetPath(tier);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public static bool Exists(int tier)
    {
        return File.Exists(GetPath(tier));
    }

    static string SkinDirectory => Path.Combine(Application.persistentDataPath, "WatermelonSkins");

    static string GetPath(int tier)
    {
        return Path.Combine(SkinDirectory, "tier_" + tier + ".png");
    }
}
