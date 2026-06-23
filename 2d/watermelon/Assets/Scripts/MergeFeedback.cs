using UnityEngine;

public class MergeFeedback : MonoBehaviour
{
    [SerializeField] AudioClip mergeClip;
    [SerializeField] AudioClip dropClip;
    [SerializeField] float mergePitchStep = 0.04f;

    AudioSource audioSource;
    int mergeCount;

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
        Color core = Saturate(color, 1.15f);
        Color bright = Color.Lerp(core, Color.white, 0.22f);
        Color dark = Color.Lerp(core, Color.black, 0.18f);

        PlayBurst(position, core, bright, dark, 12 + tier * 2, 0.28f + tier * 0.025f);
        PlayRing(position, core, 0.22f + tier * 0.02f);
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
        v = Mathf.Clamp01(v * 1.05f);
        Color result = Color.HSVToRGB(h, s, v);
        result.a = color.a;
        return result;
    }

    static void PlayBurst(Vector3 position, Color core, Color bright, Color dark, int count, float size)
    {
        GameObject fxObject = new GameObject("MergeBurst");
        fxObject.transform.position = position;

        ParticleSystem particles = fxObject.AddComponent<ParticleSystem>();
        ParticleSystem.MainModule main = particles.main;
        main.duration = 0.4f;
        main.loop = false;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.25f, 0.45f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(2.2f, 3.4f);
        main.startSize = new ParticleSystem.MinMaxCurve(size * 0.65f, size);
        main.startColor = new ParticleSystem.MinMaxGradient(bright, dark);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = count;
        main.gravityModifier = 0.55f;

        ParticleSystem.EmissionModule emission = particles.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)count) });

        ParticleSystem.ShapeModule shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.06f;

        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particles.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(bright, 0f),
                new GradientColorKey(core, 0.35f),
                new GradientColorKey(dark, 1f),
            },
            new[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0.65f, 0.5f),
                new GradientAlphaKey(0f, 1f),
            }
        );
        colorOverLifetime.color = gradient;

        ParticleSystem.SizeOverLifetimeModule sizeOverLifetime = particles.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve(
            new Keyframe(0f, 1f),
            new Keyframe(1f, 0.35f)
        );
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        ParticleSystemRenderer renderer = fxObject.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = 50;

        particles.Play();
        Object.Destroy(fxObject, 1f);
    }

    static void PlayRing(Vector3 position, Color color, float radius)
    {
        GameObject fxObject = new GameObject("MergeRing");
        fxObject.transform.position = position;

        ParticleSystem particles = fxObject.AddComponent<ParticleSystem>();
        ParticleSystem.MainModule main = particles.main;
        main.duration = 0.3f;
        main.loop = false;
        main.startLifetime = 0.3f;
        main.startSpeed = 0f;
        main.startSize = radius;
        main.startColor = new Color(color.r, color.g, color.b, 0.55f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 1;

        ParticleSystem.EmissionModule emission = particles.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 1) });

        ParticleSystem.SizeOverLifetimeModule sizeOverLifetime = particles.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve expand = new AnimationCurve(
            new Keyframe(0f, 0.4f),
            new Keyframe(1f, 2.2f)
        );
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, expand);

        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particles.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[] { new GradientColorKey(color, 0f), new GradientColorKey(color, 1f) },
            new[] { new GradientAlphaKey(0.55f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;

        ParticleSystemRenderer renderer = fxObject.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = 49;

        particles.Play();
        Object.Destroy(fxObject, 0.6f);
    }
}
