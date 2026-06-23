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
    [SerializeField] float dropHeight = 4.2f;
    [SerializeField] float wallInnerX = 2.38f;
    [SerializeField] float fruitIconScale = 0.68f;
    [SerializeField] int maxSpawnTier = 4;

    float leftBound => -wallInnerX;
    float rightBound => wallInnerX;

    [Header("UI")]
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] Text nextFruitText;
    [SerializeField] Image nextFruitIcon;
    [SerializeField] Image nextFruitIconInner;

    Sprite[] fruitIcons;
    Sprite circleFrameSprite;
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
            GameObject iconObject = GameObject.Find("NextFruitIcon");
            if (iconObject != null)
            {
                nextFruitIcon = iconObject.GetComponent<Image>();
                Transform inner = iconObject.transform.Find("Icon");
                if (inner != null)
                {
                    nextFruitIconInner = inner.GetComponent<Image>();
                }
            }
        }

        UpdateNextFruitUI();
        SpawnPreviewFruit();
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

        CreateFruit(newTier, position, false);
    }

    Fruit CreateFruit(int tier, Vector3 position, bool preview)
    {
        FruitDefinition def = database.Get(tier);
        GameObject go = new GameObject("Fruit_" + def.name);
        go.transform.SetParent(fruitContainer, false);
        go.transform.position = position;
        go.transform.localScale = Vector3.one;
        go.tag = "Fruit";

        CircleCollider2D collider = go.AddComponent<CircleCollider2D>();
        collider.radius = def.radius;
        collider.sharedMaterial = fruitPhysicsMaterial;

        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.gravityScale = 1.2f;

        FruitVisual visual = go.AddComponent<FruitVisual>();
        visual.Build(circleFrameSprite, fruitIcons[tier], def.radius, tier + 10, fruitIconScale);

        if (preview)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = false;
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
            nextFruitIcon.sprite = circleFrameSprite;
        }

        if (nextFruitIconInner != null)
        {
            nextFruitIconInner.sprite = fruitIcons[nextTier];
            nextFruitIconInner.enabled = nextFruitIconInner.sprite != null;
        }
    }

    void BuildSprites()
    {
        circleFrameSprite = SpriteFactory.CreateSuikaCircleFrame(new Color(1f, 0.98f, 0.94f));

        fruitIcons = new Sprite[database.fruits.Length];

        for (int i = 0; i < database.fruits.Length; i++)
        {
            FruitDefinition def = database.fruits[i];
            Sprite icon = FruitSpriteLoader.Load(def.name);
            fruitIcons[i] = icon != null ? icon : SpriteFactory.CreateCircleSprite(def.color);
        }
    }
}
