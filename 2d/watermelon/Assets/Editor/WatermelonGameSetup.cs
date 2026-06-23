using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class WatermelonGameSetup
{
    const string ScenePath = "Assets/Scenes/SampleScene.unity";
    const string DatabasePath = "Assets/Data/FruitDatabase.asset";
    const string PhysicsMaterialPath = "Assets/Data/FruitPhysicsMaterial.physicsMaterial2D";

    [MenuItem("Watermelon/Setup Game Scene")]
    public static void SetupGameScene()
    {
        EnsureFolders();
        FruitDatabase database = EnsureDatabase();
        PhysicsMaterial2D physicsMaterial = EnsurePhysicsMaterial();

        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        ClearSceneExceptCamera();

        Camera camera = Camera.main;
        if (camera == null)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            camera = cameraObject.AddComponent<Camera>();
            cameraObject.tag = "MainCamera";
            cameraObject.AddComponent<AudioListener>();
        }

        camera.orthographic = true;
        camera.orthographicSize = 4f;
        camera.transform.position = new Vector3(0f, 0f, -10f);
        camera.backgroundColor = new Color(0.96f, 0.93f, 0.86f);

        CreateBackground();
        CreateContainer();
        CreateGameOverZone(out GameOverDetector detector);
        CreateUI(out ScoreBoard scoreBoard, out GameObject gameOverPanel, out Text nextFruitText, out Image nextFruitIcon);
        GameManager manager = CreateGameManager(database, scoreBoard, detector, physicsMaterial, gameOverPanel, nextFruitText, nextFruitIcon);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();

        Debug.Log("Watermelon game scene setup complete. Press Play to test.");
    }

    static void EnsureFolders()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Scripts"))
        {
            AssetDatabase.CreateFolder("Assets", "Scripts");
        }

        if (!AssetDatabase.IsValidFolder("Assets/Data"))
        {
            AssetDatabase.CreateFolder("Assets", "Data");
        }
    }

    static FruitDatabase EnsureDatabase()
    {
        FruitDatabase database = AssetDatabase.LoadAssetAtPath<FruitDatabase>(DatabasePath);
        if (database == null)
        {
            database = ScriptableObject.CreateInstance<FruitDatabase>();
            database.fruits = FruitDatabase.DefaultFruits();
            AssetDatabase.CreateAsset(database, DatabasePath);
        }

        return database;
    }

    static PhysicsMaterial2D EnsurePhysicsMaterial()
    {
        PhysicsMaterial2D material = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(PhysicsMaterialPath);
        if (material == null)
        {
            material = new PhysicsMaterial2D("FruitPhysicsMaterial")
            {
                friction = 0.12f,
                bounciness = 0.28f
            };
            AssetDatabase.CreateAsset(material, PhysicsMaterialPath);
        }

        return material;
    }

    static void ClearSceneExceptCamera()
    {
        foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (root.GetComponent<Camera>() == null)
            {
                Object.DestroyImmediate(root);
            }
        }
    }

    static void CreateBackground()
    {
        GameObject background = new GameObject("Background");
        SpriteRenderer renderer = background.AddComponent<SpriteRenderer>();
        renderer.sprite = SpriteFactory.CreateCircleSprite(new Color(0.92f, 0.88f, 0.78f), 8);
        renderer.drawMode = SpriteDrawMode.Sliced;
        renderer.size = new Vector2(4.8f, 7.2f);
        renderer.sortingOrder = -10;
        background.transform.localScale = new Vector3(4.8f, 7.2f, 1f);
    }

    static void CreateContainer()
    {
        GameObject walls = new GameObject("Container");

        CreateWall(walls.transform, "LeftWall", new Vector2(-2.5f, -0.2f), new Vector2(0.2f, 7.5f));
        CreateWall(walls.transform, "RightWall", new Vector2(2.5f, -0.2f), new Vector2(0.2f, 7.5f));
        CreateWall(walls.transform, "Floor", new Vector2(0f, -3.5f), new Vector2(5.4f, 0.3f));

        GameObject dangerLine = new GameObject("DangerLine");
        dangerLine.transform.SetParent(walls.transform, false);
        dangerLine.transform.position = new Vector3(0f, 2.7f, 0f);

        SpriteRenderer lineRenderer = dangerLine.AddComponent<SpriteRenderer>();
        lineRenderer.sprite = SpriteFactory.CreateCircleSprite(new Color(0.9f, 0.25f, 0.25f, 0.8f), 8);
        lineRenderer.drawMode = SpriteDrawMode.Sliced;
        lineRenderer.size = new Vector2(4.8f, 0.05f);
        lineRenderer.sortingOrder = 5;
        dangerLine.transform.localScale = new Vector3(4.8f, 0.05f, 1f);
    }

    static void CreateWall(Transform parent, string name, Vector2 position, Vector2 size)
    {
        GameObject wall = new GameObject(name);
        wall.transform.SetParent(parent, false);
        wall.transform.position = position;
        wall.tag = "Wall";

        BoxCollider2D collider = wall.AddComponent<BoxCollider2D>();
        collider.size = size;

        SpriteRenderer renderer = wall.AddComponent<SpriteRenderer>();
        renderer.sprite = SpriteFactory.CreateCircleSprite(new Color(0.45f, 0.32f, 0.2f), 8);
        renderer.drawMode = SpriteDrawMode.Sliced;
        renderer.size = size;
        renderer.sortingOrder = -5;
        wall.transform.localScale = new Vector3(size.x, size.y, 1f);
    }

    static void CreateGameOverZone(out GameOverDetector detector)
    {
        GameObject zone = new GameObject("GameOverZone");
        zone.transform.position = new Vector3(0f, 3.3f, 0f);

        BoxCollider2D collider = zone.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(4.8f, 1.4f);

        detector = zone.AddComponent<GameOverDetector>();
    }

    static void CreateUI(out ScoreBoard scoreBoard, out GameObject gameOverPanel, out Text nextFruitText, out Image nextFruitIcon)
    {
        GameObject canvasObject = new GameObject("Canvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObject.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1080, 1920);
        canvasObject.AddComponent<GraphicRaycaster>();

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        GameObject scoreObject = CreateUIText(canvasObject.transform, "ScoreText", new Vector2(20f, -40f), TextAnchor.UpperLeft, 42);
        Text scoreText = scoreObject.GetComponent<Text>();
        scoreText.font = font;

        GameObject bestObject = CreateUIText(canvasObject.transform, "BestText", new Vector2(-20f, -40f), TextAnchor.UpperRight, 36);
        Text bestText = bestObject.GetComponent<Text>();
        bestText.font = font;
        RectTransform bestRect = bestObject.GetComponent<RectTransform>();
        bestRect.anchorMin = new Vector2(1f, 1f);
        bestRect.anchorMax = new Vector2(1f, 1f);
        bestRect.pivot = new Vector2(1f, 1f);
        bestRect.anchoredPosition = new Vector2(-20f, -40f);

        GameObject nextPanel = new GameObject("NextFruitPanel");
        nextPanel.transform.SetParent(canvasObject.transform, false);
        RectTransform nextPanelRect = nextPanel.AddComponent<RectTransform>();
        nextPanelRect.anchorMin = new Vector2(0.5f, 1f);
        nextPanelRect.anchorMax = new Vector2(0.5f, 1f);
        nextPanelRect.pivot = new Vector2(0.5f, 1f);
        nextPanelRect.sizeDelta = new Vector2(360f, 96f);
        nextPanelRect.anchoredPosition = new Vector2(0f, -110f);

        Image nextPanelBg = nextPanel.AddComponent<Image>();
        nextPanelBg.color = new Color(1f, 1f, 1f, 0.72f);

        GameObject iconObject = new GameObject("NextFruitIcon");
        iconObject.transform.SetParent(nextPanel.transform, false);
        RectTransform iconRect = iconObject.AddComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0f, 0.5f);
        iconRect.anchorMax = new Vector2(0f, 0.5f);
        iconRect.pivot = new Vector2(0f, 0.5f);
        iconRect.sizeDelta = new Vector2(72f, 72f);
        iconRect.anchoredPosition = new Vector2(16f, 0f);
        nextFruitIcon = iconObject.AddComponent<Image>();
        nextFruitIcon.preserveAspect = true;

        GameObject nextObject = CreateUIText(nextPanel.transform, "NextFruitText", new Vector2(100f, 0f), TextAnchor.MiddleLeft, 34);
        nextFruitText = nextObject.GetComponent<Text>();
        nextFruitText.font = font;
        RectTransform nextRect = nextObject.GetComponent<RectTransform>();
        nextRect.anchorMin = new Vector2(0f, 0.5f);
        nextRect.anchorMax = new Vector2(1f, 0.5f);
        nextRect.pivot = new Vector2(0f, 0.5f);
        nextRect.sizeDelta = new Vector2(-116f, 72f);
        nextRect.anchoredPosition = new Vector2(100f, 0f);

        GameObject scoreBoardObject = new GameObject("ScoreBoard");
        scoreBoardObject.transform.SetParent(canvasObject.transform, false);
        scoreBoard = scoreBoardObject.AddComponent<ScoreBoard>();

        SerializedObject scoreBoardSO = new SerializedObject(scoreBoard);
        scoreBoardSO.FindProperty("scoreText").objectReferenceValue = scoreText;
        scoreBoardSO.FindProperty("bestText").objectReferenceValue = bestText;
        scoreBoardSO.ApplyModifiedPropertiesWithoutUndo();

        gameOverPanel = new GameObject("GameOverPanel");
        gameOverPanel.transform.SetParent(canvasObject.transform, false);
        RectTransform panelRect = gameOverPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelImage = gameOverPanel.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.55f);

        GameObject titleObject = CreateUIText(gameOverPanel.transform, "GameOverTitle", Vector2.zero, TextAnchor.MiddleCenter, 64);
        Text titleText = titleObject.GetComponent<Text>();
        titleText.font = font;
        titleText.text = "Game Over";
        RectTransform titleRect = titleObject.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.5f);
        titleRect.anchorMax = new Vector2(0.5f, 0.5f);
        titleRect.anchoredPosition = new Vector2(0f, 80f);

        GameObject buttonObject = new GameObject("RestartButton");
        buttonObject.transform.SetParent(gameOverPanel.transform, false);
        RectTransform buttonRect = buttonObject.AddComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(280f, 80f);
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = new Vector2(0f, -40f);

        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.65f, 0.35f);

        Button button = buttonObject.AddComponent<Button>();

        GameObject buttonLabelObject = CreateUIText(buttonObject.transform, "Text", Vector2.zero, TextAnchor.MiddleCenter, 34);
        Text buttonLabel = buttonLabelObject.GetComponent<Text>();
        buttonLabel.font = font;
        buttonLabel.text = "Restart";
        buttonLabel.color = Color.white;
        RectTransform buttonLabelRect = buttonLabelObject.GetComponent<RectTransform>();
        buttonLabelRect.anchorMin = Vector2.zero;
        buttonLabelRect.anchorMax = Vector2.one;
        buttonLabelRect.offsetMin = Vector2.zero;
        buttonLabelRect.offsetMax = Vector2.zero;

        gameOverPanel.SetActive(false);
    }

    static GameObject CreateUIText(Transform parent, string name, Vector2 anchoredPosition, TextAnchor alignment, int fontSize)
    {
        GameObject textObject = new GameObject(name);
        textObject.transform.SetParent(parent, false);

        RectTransform rect = textObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.sizeDelta = new Vector2(500f, 80f);
        rect.anchoredPosition = anchoredPosition;

        Text text = textObject.AddComponent<Text>();
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = new Color(0.2f, 0.15f, 0.1f);
        text.text = "";

        return textObject;
    }

    static GameManager CreateGameManager(
        FruitDatabase database,
        ScoreBoard scoreBoard,
        GameOverDetector detector,
        PhysicsMaterial2D physicsMaterial,
        GameObject gameOverPanel,
        Text nextFruitText,
        Image nextFruitIcon)
    {
        GameObject managerObject = new GameObject("GameManager");
        GameManager manager = managerObject.AddComponent<GameManager>();
        MergeFeedback mergeFeedback = managerObject.AddComponent<MergeFeedback>();

        GameObject fruitContainer = new GameObject("Fruits");
        fruitContainer.transform.SetParent(managerObject.transform, false);

        SerializedObject managerSO = new SerializedObject(manager);
        SerializedProperty databaseProperty = managerSO.FindProperty("database");
        if (databaseProperty != null)
        {
            databaseProperty.objectReferenceValue = database;
        }
        else
        {
            Debug.LogError("Could not find GameManager.database property.");
        }

        managerSO.FindProperty("scoreBoard").objectReferenceValue = scoreBoard;
        managerSO.FindProperty("gameOverDetector").objectReferenceValue = detector;
        managerSO.FindProperty("fruitContainer").objectReferenceValue = fruitContainer.transform;
        managerSO.FindProperty("fruitPhysicsMaterial").objectReferenceValue = physicsMaterial;
        managerSO.FindProperty("gameOverPanel").objectReferenceValue = gameOverPanel;
        managerSO.FindProperty("nextFruitText").objectReferenceValue = nextFruitText;
        managerSO.FindProperty("nextFruitIcon").objectReferenceValue = nextFruitIcon;
        managerSO.FindProperty("mergeFeedback").objectReferenceValue = mergeFeedback;
        managerSO.ApplyModifiedProperties();
        EditorUtility.SetDirty(manager);

        EnsureFruitTag();
        return manager;
    }

    static void EnsureFruitTag()
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tags = tagManager.FindProperty("tags");

        bool hasFruitTag = false;
        bool hasWallTag = false;
        for (int i = 0; i < tags.arraySize; i++)
        {
            string tag = tags.GetArrayElementAtIndex(i).stringValue;
            if (tag == "Fruit") hasFruitTag = true;
            if (tag == "Wall") hasWallTag = true;
        }

        if (!hasFruitTag)
        {
            tags.InsertArrayElementAtIndex(tags.arraySize);
            tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = "Fruit";
        }

        if (!hasWallTag)
        {
            tags.InsertArrayElementAtIndex(tags.arraySize);
            tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = "Wall";
        }

        tagManager.ApplyModifiedProperties();
    }
}
