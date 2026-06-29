using NUnit.Framework;
using UnityEngine;

public class FruitSpriteLoaderTests
{
    [SetUp]
    public void SetUp()
    {
        UserFruitSkinStorage.Delete(0);
    }

    [TearDown]
    public void TearDown()
    {
        UserFruitSkinStorage.Delete(0);
    }

    [Test]
    public void LoadForTierUsesSavedUserImageBeforeDefaultResource()
    {
        UserFruitSkinStorage.Save(0, CreatePng(Color.magenta));

        Sprite sprite = FruitSpriteLoader.LoadForTier(0, "Cherry");

        Assert.NotNull(sprite);
        Assert.AreEqual(2, sprite.texture.width);
        Assert.AreEqual(2, sprite.texture.height);
    }

    [Test]
    public void LoadForTierFallsBackToExistingResourceLookup()
    {
        UserFruitSkinStorage.Delete(0);

        Sprite sprite = FruitSpriteLoader.LoadForTier(0, "MissingFruitName");

        Assert.Null(sprite);
    }

    static byte[] CreatePng(Color color)
    {
        Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        texture.SetPixels(new[] { color, color, color, color });
        texture.Apply();
        return texture.EncodeToPNG();
    }
}
