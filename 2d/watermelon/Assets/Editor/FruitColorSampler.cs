using System.IO;
using UnityEditor;
using UnityEngine;

public static class FruitColorSampler
{
    public static Color SampleDominantColor(string texturePath)
    {
        TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
        if (importer == null)
        {
            return Color.magenta;
        }

        bool restoreReadable = !importer.isReadable;
        if (restoreReadable)
        {
            importer.isReadable = true;
            importer.SaveAndReimport();
        }

        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(texturePath);
        Color color = SampleSprite(texture, sprite);

        if (restoreReadable)
        {
            importer.isReadable = false;
            importer.SaveAndReimport();
        }

        return color;
    }

    public static Color SampleSprite(Texture2D texture, Sprite sprite)
    {
        if (texture == null || sprite == null)
        {
            return Color.magenta;
        }

        Rect rect = sprite.textureRect;
        float centerX = rect.x + rect.width * 0.5f;
        float centerY = rect.y + rect.height * 0.5f;
        float maxRadius = Mathf.Min(rect.width, rect.height) * 0.38f;
        float maxRadiusSq = maxRadius * maxRadius;

        float rSum = 0f;
        float gSum = 0f;
        float bSum = 0f;
        float weightSum = 0f;

        int xMin = Mathf.FloorToInt(rect.x);
        int yMin = Mathf.FloorToInt(rect.y);
        int xMax = Mathf.CeilToInt(rect.xMax);
        int yMax = Mathf.CeilToInt(rect.yMax);

        for (int y = yMin; y < yMax; y++)
        {
            for (int x = xMin; x < xMax; x++)
            {
                Color pixel = texture.GetPixel(x, y);
                if (pixel.a < 0.4f)
                {
                    continue;
                }

                float dx = x - centerX;
                float dy = y - centerY;
                if (dx * dx + dy * dy > maxRadiusSq)
                {
                    continue;
                }

                float brightness = (pixel.r + pixel.g + pixel.b) / 3f;
                if (brightness > 0.9f || brightness < 0.12f)
                {
                    continue;
                }

                float max = Mathf.Max(pixel.r, Mathf.Max(pixel.g, pixel.b));
                float min = Mathf.Min(pixel.r, Mathf.Min(pixel.g, pixel.b));
                float saturation = max > 0.001f ? (max - min) / max : 0f;
                if (saturation < 0.08f)
                {
                    continue;
                }

                float weight = saturation * pixel.a;
                rSum += pixel.r * weight;
                gSum += pixel.g * weight;
                bSum += pixel.b * weight;
                weightSum += weight;
            }
        }

        if (weightSum <= 0.001f)
        {
            return Color.magenta;
        }

        return new Color(rSum / weightSum, gSum / weightSum, bSum / weightSum, 1f);
    }

    public static void UpdateFruitDatabaseColors()
    {
        FruitDatabase database = AssetDatabase.LoadAssetAtPath<FruitDatabase>("Assets/Data/FruitDatabase.asset");
        if (database == null || database.fruits == null)
        {
            Debug.LogError("FruitDatabase not found.");
            return;
        }

        int updated = 0;
        foreach (FruitDefinition fruit in database.fruits)
        {
            string fileName = fruit.name.ToLowerInvariant() + ".png";
            string path = Path.Combine("Assets/Resources/Fruits", fileName);
            if (!File.Exists(path))
            {
                continue;
            }

            Color sampled = SampleDominantColor(path);
            fruit.color = sampled;
            updated++;
        }

        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();
        Debug.Log($"Sampled dominant colors for {updated} fruits.");
    }
}
