using UnityEngine;

public class GameOverDetector : MonoBehaviour
{
    [SerializeField] float gameOverDelay = 1.5f;

    float timer;
    bool triggered;

    public System.Action OnGameOver;

    void OnTriggerStay2D(Collider2D other)
    {
        if (triggered)
        {
            return;
        }

        Fruit fruit = other.GetComponent<Fruit>();
        if (fruit == null || fruit.IsPreview || !fruit.CanMerge)
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer >= gameOverDelay)
        {
            triggered = true;
            OnGameOver?.Invoke();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Fruit>() != null)
        {
            timer = 0f;
        }
    }

    public void ResetDetector()
    {
        timer = 0f;
        triggered = false;
    }
}
