using UnityEngine;

public class FruitVisual : MonoBehaviour
{
    public void Build(Sprite frameSprite, Sprite iconSprite, float worldRadius, int sortingOrder, float iconScale = 0.68f)
    {
        float diameter = worldRadius * 2f;

        Transform frame = CreateChild("Frame", frameSprite, sortingOrder, diameter);
        CreateChild("Icon", iconSprite, sortingOrder + 1, diameter * iconScale);

        frame.localPosition = Vector3.zero;
    }

    Transform CreateChild(string childName, Sprite sprite, int sortingOrder, float diameter)
    {
        GameObject child = new GameObject(childName);
        child.transform.SetParent(transform, false);

        SpriteRenderer renderer = child.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = sortingOrder;
        child.transform.localScale = Vector3.one * diameter;

        return child.transform;
    }
}
