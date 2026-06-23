using UnityEditor;
using UnityEngine;

public class FruitTextureImporter : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        if (!assetPath.Contains("Assets/Resources/Fruits/"))
        {
            return;
        }

        TextureImporter importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.filterMode = FilterMode.Bilinear;
        importer.spritePixelsPerUnit = 512f;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.spriteMeshType = SpriteMeshType.FullRect;
    }
}
