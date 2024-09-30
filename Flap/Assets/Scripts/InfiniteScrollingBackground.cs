using UnityEngine;

public class InfiniteScrollingBackground : MonoBehaviour
{
    public GameObject[] backgrounds;  // Array of background objects
    private float backgroundWidth;    // Width of the background object

    void Start()
    {
        // Assuming all background objects are the same size, we get the width of the first one
        backgroundWidth = backgrounds[0].GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        // Check if the first background has moved off-screen
        if (backgrounds[0].transform.position.x < -backgroundWidth)
        {
            // Reposition the first background at the end of the last background
            RepositionBackground();
        }
    }

    void RepositionBackground()
    {
        // Move the first background to the end of the sequence
        GameObject firstBackground = backgrounds[0];

        // Shift the rest of the backgrounds in the array
        for (int i = 0; i < backgrounds.Length - 1; i++)
        {
            backgrounds[i] = backgrounds[i + 1];
        }

        // Place the first background at the end of the last background
        float newXPosition = backgrounds[backgrounds.Length - 1].transform.position.x + backgroundWidth;
        firstBackground.transform.position = new Vector3(newXPosition, firstBackground.transform.position.y, firstBackground.transform.position.z);

        // Put the first background back at the end of the array
        backgrounds[backgrounds.Length - 1] = firstBackground;
    }
}
