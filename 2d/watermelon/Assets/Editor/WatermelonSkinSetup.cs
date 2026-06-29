using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class WatermelonSkinSetup
{
    const string DatabasePath = "Assets/Data/FruitDatabase.asset";

    [MenuItem("Watermelon/Add Skin Setup Panel")]
    public static void AddSkinSetupPanel()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        GameManager manager = Object.FindFirstObjectByType<GameManager>();
        if (canvas == null || manager == null)
        {
            Debug.LogError("Canvas or GameManager not found. Run Watermelon → Setup Game Scene first.");
            return;
        }

        GameObject existing = GameObject.Find("FruitSkinSetupPanel");
        if (existing != null)
        {
            Object.DestroyImmediate(existing);
        }

        FruitDatabase database = AssetDatabase.LoadAssetAtPath<FruitDatabase>(DatabasePath);
        if (database == null)
        {
            database = ScriptableObject.CreateInstance<FruitDatabase>();
            database.fruits = FruitDatabase.DefaultFruits();
            AssetDatabase.CreateAsset(database, DatabasePath);
        }

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        GameObject panel = CreatePanel(canvas.transform);
        FruitSkinSetupPanel setupPanel = panel.AddComponent<FruitSkinSetupPanel>();

        Image[] previews = new Image[database.fruits.Length];
        Text[] names = new Text[database.fruits.Length];
        Button[] uploadButtons = new Button[database.fruits.Length];
        Button[] defaultButtons = new Button[database.fruits.Length];

        GameObject title = CreateText(panel.transform, "Title", "Customize Fruits", font, 54, TextAnchor.MiddleCenter);
        title.GetComponent<LayoutElement>().preferredHeight = 80f;

        for (int i = 0; i < database.fruits.Length; i++)
        {
            CreateFruitRow(panel.transform, database.fruits[i].name, font, out names[i], out previews[i], out uploadButtons[i], out defaultButtons[i]);
        }

        GameObject actions = new GameObject("Actions");
        actions.transform.SetParent(panel.transform, false);
        HorizontalLayoutGroup actionsLayout = actions.AddComponent<HorizontalLayoutGroup>();
        actionsLayout.spacing = 20f;
        actionsLayout.childAlignment = TextAnchor.MiddleCenter;
        actions.AddComponent<LayoutElement>().preferredHeight = 90f;

        Button resetAllButton = CreateButton(actions.transform, "ResetAllButton", "All Default", font);
        Button startButton = CreateButton(actions.transform, "StartButton", "Start Game", font);

        SerializedObject serialized = new SerializedObject(setupPanel);
        serialized.FindProperty("database").objectReferenceValue = database;
        serialized.FindProperty("gameManager").objectReferenceValue = manager;
        SetObjectArray(serialized.FindProperty("previewImages"), previews);
        SetObjectArray(serialized.FindProperty("nameTexts"), names);
        SetObjectArray(serialized.FindProperty("uploadButtons"), uploadButtons);
        SetObjectArray(serialized.FindProperty("defaultButtons"), defaultButtons);
        serialized.FindProperty("startButton").objectReferenceValue = startButton;
        serialized.FindProperty("resetAllButton").objectReferenceValue = resetAllButton;
        serialized.ApplyModifiedPropertiesWithoutUndo();

        EditorUtility.SetDirty(panel);
        EditorSceneManager.MarkSceneDirty(panel.scene);
        Debug.Log("Fruit skin setup panel added. It will pause fruit spawning until Start Game is clicked.");
    }

    static GameObject CreatePanel(Transform parent)
    {
        GameObject panel = new GameObject("FruitSkinSetupPanel");
        panel.transform.SetParent(parent, false);

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image background = panel.AddComponent<Image>();
        background.color = new Color(0.96f, 0.93f, 0.86f, 0.98f);

        VerticalLayoutGroup layout = panel.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(80, 80, 80, 80);
        layout.spacing = 12f;
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;

        return panel;
    }

    static void CreateFruitRow(Transform parent, string fruitName, Font font, out Text nameText, out Image preview, out Button uploadButton, out Button defaultButton)
    {
        GameObject row = new GameObject(fruitName + "Row");
        row.transform.SetParent(parent, false);
        HorizontalLayoutGroup layout = row.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 16f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        row.AddComponent<LayoutElement>().preferredHeight = 86f;

        GameObject previewObject = new GameObject("Preview");
        previewObject.transform.SetParent(row.transform, false);
        preview = previewObject.AddComponent<Image>();
        preview.preserveAspect = true;
        previewObject.AddComponent<LayoutElement>().preferredWidth = 80f;

        GameObject label = CreateText(row.transform, "Name", fruitName, font, 30, TextAnchor.MiddleLeft);
        label.GetComponent<LayoutElement>().flexibleWidth = 1f;
        nameText = label.GetComponent<Text>();

        uploadButton = CreateButton(row.transform, "UploadButton", "Upload", font);
        defaultButton = CreateButton(row.transform, "DefaultButton", "Default", font);
    }

    static GameObject CreateText(Transform parent, string name, string value, Font font, int size, TextAnchor alignment)
    {
        GameObject textObject = new GameObject(name);
        textObject.transform.SetParent(parent, false);
        Text text = textObject.AddComponent<Text>();
        text.text = value;
        text.font = font;
        text.fontSize = size;
        text.alignment = alignment;
        text.color = new Color(0.2f, 0.15f, 0.1f);
        textObject.AddComponent<LayoutElement>();
        return textObject;
    }

    static Button CreateButton(Transform parent, string name, string value, Font font)
    {
        GameObject buttonObject = new GameObject(name);
        buttonObject.transform.SetParent(parent, false);
        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.2f, 0.65f, 0.35f);
        Button button = buttonObject.AddComponent<Button>();
        buttonObject.AddComponent<LayoutElement>().preferredWidth = 180f;

        GameObject textObject = CreateText(buttonObject.transform, "Text", value, font, 28, TextAnchor.MiddleCenter);
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        textObject.GetComponent<Text>().color = Color.white;
        return button;
    }

    static void SetObjectArray<T>(SerializedProperty property, T[] values) where T : Object
    {
        property.arraySize = values.Length;
        for (int i = 0; i < values.Length; i++)
        {
            property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
        }
    }
}
