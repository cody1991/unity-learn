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
        PlayBurst(position, color, 14 + tier * 2, 0.35f + tier * 0.03f);
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

    static void PlayBurst(Vector3 position, Color color, int count, float size)
    {
        GameObject fxObject = new GameObject("MergeBurst");
        fxObject.transform.position = position;

        ParticleSystem particles = fxObject.AddComponent<ParticleSystem>();
        ParticleSystem.MainModule main = particles.main;
        main.duration = 0.35f;
        main.loop = false;
        main.startLifetime = 0.35f;
        main.startSpeed = 2.8f;
        main.startSize = size;
        main.startColor = color;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = count;
        main.gravityModifier = 0.6f;

        ParticleSystem.EmissionModule emission = particles.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)count) });

        ParticleSystem.ShapeModule shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.08f;

        ParticleSystemRenderer renderer = fxObject.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = 50;

        particles.Play();
        Object.Destroy(fxObject, 1f);
    }
}
