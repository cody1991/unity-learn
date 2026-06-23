using UnityEngine;

public static class SpriteFactory
{
    public static Sprite CreateCircleSprite(Color color, int resolution = 128)
    {
        return CreateCircleSpriteInternal(color, color * 0.7f, resolution);
    }

    public static Sprite CreateSuikaCircleFrame(Color fillColor, int resolution = 128)
    {
        Color rimColor = new Color(0.32f, 0.2f, 0.12f, 1f);
        return CreateCircleSpriteInternal(fillColor, rimColor, resolution, rimWidth: 5f, highlight: true);
    }

    public static Sprite CreateCircleMaskSprite(int resolution = 128)
    {
        return CreateCircleSpriteInternal(Color.white, Color.white, resolution, rimWidth: 0f, highlight: false);
    }

    static Sprite CreateCircleSpriteInternal(
        Color fillColor,
        Color rimColor,
        int resolution = 128,
        float rimWidth = 2.5f,
        bool highlight = false)
    {
        var texture = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };

        float center = resolution * 0.5f;
        float outerRadius = center - 2f;
        float innerFillRadius = outerRadius - rimWidth;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float dist = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(center, center));

                if (dist > outerRadius)
                {
                    texture.SetPixel(x, y, Color.clear);
                }
                else if (dist > innerFillRadius)
                {
                    texture.SetPixel(x, y, rimColor);
                }
                else if (highlight)
                {
                    float nx = (x - center) / outerRadius;
                    float ny = (y - center) / outerRadius;
                    float shade = 1f - Mathf.Clamp01((nx + ny) * 0.12f);
                    texture.SetPixel(x, y, fillColor * shade);
                }
                else
                {
                    texture.SetPixel(x, y, fillColor);
                }
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, resolution, resolution), new Vector2(0.5f, 0.5f), resolution);
    }
}
