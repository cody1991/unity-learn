using UnityEditor;
using UnityEngine;

public static class FruitSpriteFixer
{
    [MenuItem("Watermelon/Fix Fruit Sprite Import")]
    public static void FixFruitSprites()
    {
        string[] guids = AssetDatabase.FindAssets(string.Empty, new[] { "Assets/Resources/Fruits" });
        int count = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (!path.EndsWith(".png"))
            {
                continue;
            }

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                continue;
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 512f;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.filterMode = FilterMode.Bilinear;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
            count++;
        }

        Debug.Log($"Reimported {count} fruit sprites at 512 PPU.");
    }
}
