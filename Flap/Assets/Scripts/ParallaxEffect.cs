using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    // Reference to the player's transform
    public Transform player;

    // The parallax effect multiplier for this layer
    public float parallaxFactorX = 0.5f; // For horizontal parallax (if needed)
    public float parallaxFactorY = 0.5f; // For vertical parallax (uphill movement)

    // The initial position of the background
    private Vector3 initialPosition;

    void Start()
    {
        // Store the initial position of the background
        initialPosition = transform.position;
    }

    void Update()
    {
        // Calculate the difference in player's position
        Vector3 playerMovement = player.position;

        // Modify the background's position based on the player's movement and the parallax factor
        transform.position = new Vector3(
            initialPosition.x + playerMovement.x * parallaxFactorX,
            initialPosition.y + playerMovement.y * parallaxFactorY,
            initialPosition.z
        );
    }
}
