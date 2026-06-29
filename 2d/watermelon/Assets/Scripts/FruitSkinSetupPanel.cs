using System;
using UnityEngine;
using UnityEngine.UI;

public class FruitSkinSetupPanel : MonoBehaviour
{
    [SerializeField] FruitDatabase database;
    [SerializeField] GameManager gameManager;
    [SerializeField] Image[] previewImages;
    [SerializeField] Text[] nameTexts;
    [SerializeField] Button[] uploadButtons;
    [SerializeField] Button[] defaultButtons;
    [SerializeField] Button startButton;
    [SerializeField] Button resetAllButton;

    void Start()
    {
        EnsureReferences();
        HookButtons();
        RefreshAll();
    }

    void EnsureReferences()
    {
        if (database == null)
        {
            database = Resources.Load<FruitDatabase>("FruitDatabase");
        }

        if (database == null || database.fruits == null || database.fruits.Length == 0)
        {
            database = ScriptableObject.CreateInstance<FruitDatabase>();
            database.fruits = FruitDatabase.DefaultFruits();
        }

        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }
    }

    void HookButtons()
    {
        for (int i = 0; i < uploadButtons.Length; i++)
        {
            int tier = i;
            if (uploadButtons[i] != null)
            {
                uploadButtons[i].onClick.AddListener(() => UploadTier(tier));
            }

            if (i < defaultButtons.Length && defaultButtons[i] != null)
            {
                defaultButtons[i].onClick.AddListener(() => RestoreDefault(tier));
            }
        }

        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
        }

        if (resetAllButton != null)
        {
            resetAllButton.onClick.AddListener(RestoreAllDefaults);
        }
    }

    public void UploadTier(int tier)
    {
        WebGLImagePicker.Open(tier, gameObject.name, nameof(ReceiveUploadedImage));
    }

    public void ReceiveUploadedImage(string payload)
    {
        string[] parts = payload.Split(new[] { '|' }, 2);
        if (parts.Length != 2 || !int.TryParse(parts[0], out int tier))
        {
            Debug.LogWarning("Invalid fruit image upload payload.");
            return;
        }

        try
        {
            byte[] imageBytes = Convert.FromBase64String(parts[1]);
            UserFruitSkinStorage.Save(tier, imageBytes);
            RefreshTier(tier);
        }
        catch (FormatException)
        {
            Debug.LogWarning("Uploaded fruit image was not valid base64.");
        }
    }

    public void RestoreDefault(int tier)
    {
        UserFruitSkinStorage.Delete(tier);
        RefreshTier(tier);
    }

    public void RestoreAllDefaults()
    {
        for (int i = 0; i < database.fruits.Length; i++)
        {
            UserFruitSkinStorage.Delete(i);
        }

        RefreshAll();
    }

    public void StartGame()
    {
        gameObject.SetActive(false);
        gameManager?.BeginGame();
    }

    void RefreshAll()
    {
        for (int i = 0; i < database.fruits.Length; i++)
        {
            RefreshTier(i);
        }
    }

    void RefreshTier(int tier)
    {
        if (tier < 0 || tier >= database.fruits.Length)
        {
            return;
        }

        FruitDefinition def = database.fruits[tier];
        if (tier < nameTexts.Length && nameTexts[tier] != null)
        {
            nameTexts[tier].text = (tier + 1) + ". " + def.name;
        }

        if (tier < previewImages.Length && previewImages[tier] != null)
        {
            Sprite sprite = FruitSpriteLoader.LoadForTier(tier, def.name);
            previewImages[tier].sprite = sprite != null ? sprite : SpriteFactory.CreateCircleSprite(def.color);
            previewImages[tier].preserveAspect = true;
        }
    }
}
