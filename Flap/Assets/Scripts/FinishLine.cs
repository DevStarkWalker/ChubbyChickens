using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private int totalPlayers;
    private int playersCrossed;

    private void Start()
    {
        totalPlayers = GameManager.Instance.playerCount;
        playersCrossed = 0;
    }

    private void Update()
    {
        if(playersCrossed >= totalPlayers) 
        { 
            GameManager.Instance.FinishedLevel();
            this.gameObject.SetActive(false); 
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playersCrossed++;
        }
    }
}
