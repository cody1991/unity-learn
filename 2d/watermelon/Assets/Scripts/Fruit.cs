using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class Fruit : MonoBehaviour
{
    public int Tier { get; private set; }
    public bool CanMerge { get; private set; }
    public bool IsPreview { get; private set; }

    bool isMerging;
    float mergeEnableTime = 0.35f;

    public void Setup(int tier, bool preview)
    {
        Tier = tier;
        IsPreview = preview;
        CanMerge = false;

        if (!preview)
        {
            Invoke(nameof(EnableMerge), mergeEnableTime);
        }
    }

    void EnableMerge()
    {
        CanMerge = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!CanMerge || isMerging || IsPreview)
        {
            return;
        }

        Fruit other = collision.gameObject.GetComponent<Fruit>();
        if (other == null || !other.CanMerge || other.isMerging || other.IsPreview)
        {
            return;
        }

        if (other.Tier != Tier)
        {
            return;
        }

        if (GetInstanceID() > other.GetInstanceID())
        {
            return;
        }

        isMerging = true;
        other.isMerging = true;

        Vector3 mergePosition = (transform.position + other.transform.position) * 0.5f;
        int newTier = Tier + 1;

        GameManager.Instance.HandleMerge(newTier, mergePosition);

        Destroy(other.gameObject);
        Destroy(gameObject);
    }
}
