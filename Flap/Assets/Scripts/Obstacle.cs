using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float force;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D colRB = collision.rigidbody;

        if (collision.transform.CompareTag("Player")) {

            Debug.Log("hit player");

            // Apply force in the direction of the collision
            if (colRB != null)
            {
                Vector2 direction = collision.contacts[0].normal; // Get the normal of the collision contact point
                colRB.AddForce(-direction * force * 5, ForceMode2D.Impulse); // Apply force in the opposite direction
            }
        }
        if (collision.transform.CompareTag("Obstacle"))
        {
            // Apply force in the direction of the collision
            if (colRB != null)
            {
                Vector2 direction = collision.contacts[0].normal; // Get the normal of the collision contact point
                colRB.AddForce(-direction * force, ForceMode2D.Impulse); // Apply force in the opposite direction
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Floor"))
        {
            Debug.Log("Destroy this object: " + collision.name);
        }
    }
}
