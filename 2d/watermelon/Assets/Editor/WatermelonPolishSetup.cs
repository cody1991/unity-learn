using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class WatermelonPolishSetup
{
    [MenuItem("Watermelon/Apply Polish To Scene")]
    public static void ApplyPolishToScene()
    {
        AssetDatabase.ImportAsset("Assets/Resources/Fruits", ImportAssetOptions.ImportRecursive);
        AssetDatabase.ImportAsset("Assets/Resources/Audio", ImportAssetOptions.ImportRecursive);

        GameManager manager = Object.FindFirstObjectByType<GameManager>();
        if (manager == null)
        {
            Debug.LogError("GameManager not found. Run Watermelon → Setup Game Scene first.");
            return;
        }

        MergeFeedback feedback = manager.GetComponent<MergeFeedback>();
        if (feedback == null)
        {
            feedback = manager.gameObject.AddComponent<MergeFeedback>();
        }

        Image nextFruitIcon = EnsureNextFruitIcon(manager);
        Image nextFruitIconInner = EnsureNextFruitIconInner(nextFruitIcon);

        SerializedObject managerSO = new SerializedObject(manager);
        managerSO.FindProperty("mergeFeedback").objectReferenceValue = feedback;
        managerSO.FindProperty("nextFruitIcon").objectReferenceValue = nextFruitIcon;
        managerSO.FindProperty("nextFruitIconInner").objectReferenceValue = nextFruitIconInner;
        managerSO.ApplyModifiedProperties();
        EditorUtility.SetDirty(manager);

        EditorSceneManager.MarkSceneDirty(manager.gameObject.scene);
        Debug.Log("Polish applied: fruit sprites, merge FX, next-fruit icon.");
    }

    static Image EnsureNextFruitIcon(GameManager manager)
    {
        Image existing = GameObject.Find("NextFruitIcon")?.GetComponent<Image>();
        if (existing != null)
        {
            EnsureNextFruitIconInner(existing);
            return existing;
        }

        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            return null;
        }

        Text nextText = null;
        foreach (Text text in canvas.GetComponentsInChildren<Text>(true))
        {
            if (text.gameObject.name == "NextFruitText")
            {
                nextText = text;
                break;
            }
        }

        GameObject panel = new GameObject("NextFruitPanel");
        panel.transform.SetParent(canvas.transform, false);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 1f);
        panelRect.anchorMax = new Vector2(0.5f, 1f);
        panelRect.pivot = new Vector2(0.5f, 1f);
        panelRect.sizeDelta = new Vector2(360f, 96f);
        panelRect.anchoredPosition = new Vector2(0f, -110f);

        Image panelBg = panel.AddComponent<Image>();
        panelBg.color = new Color(1f, 1f, 1f, 0.72f);

        GameObject iconObject = new GameObject("NextFruitIcon");
        iconObject.transform.SetParent(panel.transform, false);
        RectTransform iconRect = iconObject.AddComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0f, 0.5f);
        iconRect.anchorMax = new Vector2(0f, 0.5f);
        iconRect.pivot = new Vector2(0f, 0.5f);
        iconRect.sizeDelta = new Vector2(72f, 72f);
        iconRect.anchoredPosition = new Vector2(16f, 0f);
        Image icon = iconObject.AddComponent<Image>();
        icon.preserveAspect = true;
        EnsureNextFruitIconInner(icon);

        if (nextText != null)
        {
            nextText.transform.SetParent(panel.transform, false);
            RectTransform textRect = nextText.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0f, 0.5f);
            textRect.anchorMax = new Vector2(1f, 0.5f);
            textRect.pivot = new Vector2(0f, 0.5f);
            textRect.sizeDelta = new Vector2(-116f, 72f);
            textRect.anchoredPosition = new Vector2(100f, 0f);
            nextText.alignment = TextAnchor.MiddleLeft;
        }

        return icon;
    }

    static Image EnsureNextFruitIconInner(Image nextFruitIcon)
    {
        if (nextFruitIcon == null)
        {
            return null;
        }

        Transform existing = nextFruitIcon.transform.Find("Icon");
        if (existing != null)
        {
            return existing.GetComponent<Image>();
        }

        GameObject innerIconObject = new GameObject("Icon");
        innerIconObject.transform.SetParent(nextFruitIcon.transform, false);
        RectTransform innerIconRect = innerIconObject.AddComponent<RectTransform>();
        innerIconRect.anchorMin = Vector2.zero;
        innerIconRect.anchorMax = Vector2.one;
        innerIconRect.offsetMin = new Vector2(8f, 8f);
        innerIconRect.offsetMax = new Vector2(-8f, -8f);
        Image innerImage = innerIconObject.AddComponent<Image>();
        innerImage.preserveAspect = true;
        return innerImage;
    }
}
