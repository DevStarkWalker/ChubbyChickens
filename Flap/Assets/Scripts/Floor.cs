using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.GameOver();
        }

        if (other.gameObject.CompareTag("Obstacle"))
        {
            GameManager.Instance.OutOfBounds(other.gameObject);
        }
    }


}
