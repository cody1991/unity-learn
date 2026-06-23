using UnityEngine;

public class MergeFeedback : MonoBehaviour
{
    [SerializeField] AudioClip mergeClip;
    [SerializeField] AudioClip dropClip;
    [SerializeField] float mergePitchStep = 0.04f;

    AudioSource audioSource;
    int mergeCount;
    static Sprite particleSprite;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        if (mergeClip == null)
        {
            mergeClip = Resources.Load<AudioClip>("Audio/merge");
        }

        if (dropClip == null)
        {
            dropClip = Resources.Load<AudioClip>("Audio/drop");
        }
    }

    public void PlayMerge(Vector3 position, Color color, int tier)
    {
        Color core = Saturate(color, 1.35f);
        int count = 10 + tier * 2;
        float size = 0.14f + tier * 0.018f;

        for (int i = 0; i < count; i++)
        {
            SpawnParticle(position, core, size);
        }

        SpawnRing(position, core, 0.18f + tier * 0.02f);
        PlayMergeSound(tier);
    }

    public void PlayDrop()
    {
        if (dropClip == null || audioSource == null)
        {
            return;
        }

        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(dropClip, 0.35f);
    }

    void PlayMergeSound(int tier)
    {
        if (mergeClip == null || audioSource == null)
        {
            return;
        }

        mergeCount++;
        audioSource.pitch = 1f + tier * mergePitchStep + Mathf.Min(mergeCount * 0.01f, 0.2f);
        audioSource.PlayOneShot(mergeClip, 0.55f);
    }

    static Color Saturate(Color color, float amount)
    {
        Color.RGBToHSV(color, out float h, out float s, out float v);
        s = Mathf.Clamp01(s * amount);
        v = Mathf.Clamp01(v * 1.08f);
        Color result = Color.HSVToRGB(h, s, v);
        result.a = color.a;
        return result;
    }

    static Sprite GetParticleSprite()
    {
        if (particleSprite == null)
        {
            particleSprite = SpriteFactory.CreateCircleMaskSprite(32);
        }

        return particleSprite;
    }

    static void SpawnParticle(Vector3 origin, Color color, float size)
    {
        GameObject go = new GameObject("MergeParticle");
        go.transform.position = origin + (Vector3)Random.insideUnitCircle * 0.04f;

        SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = GetParticleSprite();
        renderer.sortingOrder = 50;

        Color tint = Color.Lerp(color, Color.white, Random.Range(0f, 0.25f));
        tint.a = 1f;
        renderer.color = tint;

        float scale = size * Random.Range(0.55f, 1.1f);
        go.transform.localScale = Vector3.one * scale;

        Vector2 velocity = Random.insideUnitCircle.normalized * Random.Range(1.4f, 3.2f);
        MergeParticleFx fx = go.AddComponent<MergeParticleFx>();
        fx.Init(velocity, tint, Random.Range(0.22f, 0.42f));
    }

    static void SpawnRing(Vector3 position, Color color, float radius)
    {
        GameObject go = new GameObject("MergeRing");
        go.transform.position = position;

        SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = SpriteFactory.CreateCircleSprite(color, 64);
        renderer.sortingOrder = 49;

        Color ringColor = color;
        ringColor.a = 0.65f;
        renderer.color = ringColor;
        go.transform.localScale = Vector3.one * radius;

        MergeRingFx fx = go.AddComponent<MergeRingFx>();
        fx.Init(ringColor, 0.32f);
    }
}

public class MergeParticleFx : MonoBehaviour
{
    Vector2 velocity;
    float lifetime;
    float elapsed;
    Color startColor;
    SpriteRenderer spriteRenderer;

    public void Init(Vector2 vel, Color color, float life)
    {
        velocity = vel;
        lifetime = life;
        startColor = color;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= lifetime)
        {
            Destroy(gameObject);
            return;
        }

        transform.position += (Vector3)(velocity * Time.deltaTime);
        velocity *= Mathf.Pow(0.9f, Time.deltaTime * 60f);

        float t = elapsed / lifetime;
        Color color = startColor;
        color.a = 1f - t;
        spriteRenderer.color = color;
    }
}

public class MergeRingFx : MonoBehaviour
{
    float lifetime;
    float elapsed;
    Color startColor;
    float startScale;
    SpriteRenderer spriteRenderer;

    public void Init(Color color, float life)
    {
        lifetime = life;
        startColor = color;
        startScale = transform.localScale.x;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= lifetime)
        {
            Destroy(gameObject);
            return;
        }

        float t = elapsed / lifetime;
        float scale = Mathf.Lerp(startScale, startScale * 2.4f, t);
        transform.localScale = Vector3.one * scale;

        Color color = startColor;
        color.a = Mathf.Lerp(startColor.a, 0f, t);
        spriteRenderer.color = color;
    }
}
