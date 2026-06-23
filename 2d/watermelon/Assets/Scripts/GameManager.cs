using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] FruitDatabase database;
    [SerializeField] ScoreBoard scoreBoard;
    [SerializeField] GameOverDetector gameOverDetector;
    [SerializeField] Transform fruitContainer;
    [SerializeField] PhysicsMaterial2D fruitPhysicsMaterial;
    [SerializeField] MergeFeedback mergeFeedback;

    [Header("Layout")]
    [SerializeField] float dropHeight = 3.4f;
    [SerializeField] float wallInnerX = 2.34f;
    [SerializeField] int maxSpawnTier = 4;

    float leftBound => -wallInnerX;
    float rightBound => wallInnerX;

    [Header("UI")]
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] Text nextFruitText;
    [SerializeField] Image nextFruitIcon;

    Sprite[] fruitSprites;
    Fruit currentFruit;
    int nextTier;
    bool isDropping;
    bool gameOver;

    void Awake()
    {
        Instance = this;
        EnsureDatabase();
        EnsureMergeFeedback();
        BuildSprites();
        nextTier = RollNextTier();
    }

    void EnsureDatabase()
    {
        if (database != null && database.fruits != null && database.fruits.Length > 0)
        {
            return;
        }

        database = Resources.Load<FruitDatabase>("FruitDatabase");
        if (database != null && database.fruits != null && database.fruits.Length > 0)
        {
            return;
        }

        database = ScriptableObject.CreateInstance<FruitDatabase>();
        database.fruits = FruitDatabase.DefaultFruits();
        Debug.LogWarning("FruitDatabase reference was missing. Using built-in fruit data.");
    }

    void EnsureMergeFeedback()
    {
        if (mergeFeedback != null)
        {
            return;
        }

        mergeFeedback = GetComponent<MergeFeedback>();
        if (mergeFeedback == null)
        {
            mergeFeedback = gameObject.AddComponent<MergeFeedback>();
        }
    }

    void Start()
    {
        if (gameOverDetector != null)
        {
            gameOverDetector.OnGameOver += HandleGameOver;
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            Button restartButton = gameOverPanel.GetComponentInChildren<Button>();
            if (restartButton != null)
            {
                restartButton.onClick.AddListener(RestartGame);
            }
        }

        if (nextFruitIcon == null)
        {
            nextFruitIcon = GameObject.Find("NextFruitIcon")?.GetComponent<Image>();
        }

        EnsureNextFruitUI();
        UpdateNextFruitUI();
        SpawnPreviewFruit();
    }

    void EnsureNextFruitUI()
    {
        if (nextFruitIcon != null)
        {
            return;
        }

        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            return;
        }

        if (nextFruitText == null)
        {
            foreach (Text text in canvas.GetComponentsInChildren<Text>(true))
            {
                if (text.gameObject.name == "NextFruitText")
                {
                    nextFruitText = text;
                    break;
                }
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
        panelBg.color = new Color(1f, 1f, 1f, 0.82f);

        GameObject iconObject = new GameObject("NextFruitIcon");
        iconObject.transform.SetParent(panel.transform, false);
        RectTransform iconRect = iconObject.AddComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0f, 0.5f);
        iconRect.anchorMax = new Vector2(0f, 0.5f);
        iconRect.pivot = new Vector2(0f, 0.5f);
        iconRect.sizeDelta = new Vector2(72f, 72f);
        iconRect.anchoredPosition = new Vector2(16f, 0f);
        nextFruitIcon = iconObject.AddComponent<Image>();
        nextFruitIcon.preserveAspect = true;

        if (nextFruitText != null)
        {
            nextFruitText.transform.SetParent(panel.transform, false);
            RectTransform textRect = nextFruitText.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0f, 0.5f);
            textRect.anchorMax = new Vector2(1f, 0.5f);
            textRect.pivot = new Vector2(0f, 0.5f);
            textRect.sizeDelta = new Vector2(-116f, 72f);
            textRect.anchoredPosition = new Vector2(100f, 0f);
            nextFruitText.alignment = TextAnchor.MiddleLeft;
        }
    }

    void OnDestroy()
    {
        if (gameOverDetector != null)
        {
            gameOverDetector.OnGameOver -= HandleGameOver;
        }

        if (Instance == this)
        {
            Instance = null;
        }
    }

    void Update()
    {
        if (gameOver || currentFruit == null || isDropping)
        {
            return;
        }

        Vector3 position = currentFruit.transform.position;
        float targetX = GetPointerWorldX();
        float radius = database.Get(currentFruit.Tier).radius;
        position.x = Mathf.Clamp(targetX, leftBound + radius, rightBound - radius);
        position.y = dropHeight;
        currentFruit.transform.position = position;

        if (WasDropPressed())
        {
            DropCurrentFruit();
        }
    }

    float GetPointerWorldX()
    {
        Vector2 screenPosition = GetPointerScreenPosition();
        Vector3 world = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, -Camera.main.transform.position.z));
        return world.x;
    }

    Vector2 GetPointerScreenPosition()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            return Touchscreen.current.primaryTouch.position.ReadValue();
        }

        if (Mouse.current != null)
        {
            return Mouse.current.position.ReadValue();
        }

        return Vector2.zero;
    }

    bool WasDropPressed()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            return true;
        }

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasReleasedThisFrame)
        {
            return true;
        }

        return false;
    }

    int RollNextTier()
    {
        return Random.Range(0, maxSpawnTier + 1);
    }

    void SpawnPreviewFruit()
    {
        currentFruit = CreateFruit(nextTier, new Vector3(0f, dropHeight, 0f), true);
        isDropping = false;
    }

    void DropCurrentFruit()
    {
        if (currentFruit == null)
        {
            return;
        }

        Rigidbody2D rb = currentFruit.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.simulated = true;

        Fruit fruit = currentFruit.GetComponent<Fruit>();
        fruit.Setup(fruit.Tier, false);
        ApplyNaturalPhysics(rb);

        mergeFeedback?.PlayDrop();

        currentFruit = null;
        isDropping = true;
        nextTier = RollNextTier();
        UpdateNextFruitUI();

        Invoke(nameof(SpawnPreviewFruit), 0.6f);
    }

    public void HandleMerge(int newTier, Vector3 position)
    {
        if (newTier > database.MaxTier)
        {
            return;
        }

        FruitDefinition mergedFrom = database.Get(newTier - 1);
        scoreBoard.AddScore(mergedFrom.mergeScore);

        if (mergeFeedback != null)
        {
            mergeFeedback.PlayMerge(position, mergedFrom.color, newTier - 1);
        }

        CreateFruit(newTier, JitterMergePosition(position), false);
    }

    Vector3 JitterMergePosition(Vector3 position)
    {
        position.x += Random.Range(-0.04f, 0.04f);
        position.y += 0.03f;
        return position;
    }

    Fruit CreateFruit(int tier, Vector3 position, bool preview)
    {
        FruitDefinition def = database.Get(tier);
        Sprite sprite = fruitSprites[tier];
        float spriteRadius = sprite.bounds.extents.x;

        GameObject go = new GameObject("Fruit_" + def.name);
        go.transform.SetParent(fruitContainer, false);
        go.transform.position = position;
        go.transform.localScale = Vector3.one * (def.radius / spriteRadius);
        go.tag = "Fruit";

        SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = tier + 10;

        CircleCollider2D collider = go.AddComponent<CircleCollider2D>();
        collider.radius = spriteRadius;
        collider.sharedMaterial = fruitPhysicsMaterial;

        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.gravityScale = 1.2f;
        rb.angularDamping = 0.15f;
        rb.linearDamping = 0.05f;

        if (preview)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = false;
        }
        else
        {
            ApplyNaturalPhysics(rb);
        }

        Fruit fruit = go.AddComponent<Fruit>();
        fruit.Setup(tier, preview);

        if (!preview)
        {
            FruitBoundaryClamp boundaryClamp = go.AddComponent<FruitBoundaryClamp>();
            boundaryClamp.Configure(wallInnerX);
        }

        return fruit;
    }

    void HandleGameOver()
    {
        if (gameOver)
        {
            return;
        }

        gameOver = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void RestartGame()
    {
        CancelInvoke(nameof(SpawnPreviewFruit));

        if (fruitContainer != null)
        {
            for (int i = fruitContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(fruitContainer.GetChild(i).gameObject);
            }
        }

        scoreBoard.ResetScore();
        gameOverDetector.ResetDetector();
        gameOver = false;
        isDropping = false;
        currentFruit = null;
        nextTier = RollNextTier();
        UpdateNextFruitUI();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        SpawnPreviewFruit();
    }

    void UpdateNextFruitUI()
    {
        FruitDefinition def = database.Get(nextTier);

        if (nextFruitText != null)
        {
            nextFruitText.text = "Next: " + def.name;
            nextFruitText.color = new Color(0.2f, 0.15f, 0.1f);
        }

        if (nextFruitIcon != null)
        {
            nextFruitIcon.sprite = fruitSprites[nextTier];
            nextFruitIcon.enabled = nextFruitIcon.sprite != null;
        }
    }

    void BuildSprites()
    {
        fruitSprites = new Sprite[database.fruits.Length];

        for (int i = 0; i < database.fruits.Length; i++)
        {
            FruitDefinition def = database.fruits[i];
            Sprite sprite = FruitSpriteLoader.Load(def.name);
            fruitSprites[i] = sprite != null ? sprite : SpriteFactory.CreateCircleSprite(def.color);
        }
    }

    void ApplyNaturalPhysics(Rigidbody2D rb)
    {
        rb.AddTorque(Random.Range(-6f, 6f), ForceMode2D.Impulse);
        rb.AddForce(new Vector2(Random.Range(-0.12f, 0.12f), 0f), ForceMode2D.Impulse);
    }
}
