using UnityEngine;

public class select_character : MonoBehaviour
{


    [SerializeField] GameObject scoreCanvas;
    [SerializeField] GameObject dragonSprite;
    [SerializeField] GameObject frogSprite;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void BeginGame() {
        Time.timeScale = 1;

        scoreCanvas.SetActive(true);
        gameObject.SetActive(false);
    }

    public void ChooseDragon() {
        Debug.Log("Choose Dragon");
        dragonSprite.SetActive(true);
        frogSprite.SetActive(false);
        BeginGame();
    }

    public void ChooseFrog() {
        Debug.Log("Choose Frog");
        frogSprite.SetActive(true);
        dragonSprite.SetActive(false);
        BeginGame();
    }
}
