using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    public GameObject[] levelPrefabs;  // Array of piece prefabs
    public int piecesToSpawn;      // Number of pieces per level segment
    public float spawnDistance = 10f;  // Distance between each piece

    private Vector3 lastPosition;      // Last position where the previous piece was placed
    private List<GameObject> currentPieces = new List<GameObject>();
    private GameObject currentFinishLine;

    void Start()
    {
        // Initialize level at the start
        BuildLevel();
    }

    // Builds the level, spawning the given number of pieces
    public void BuildLevel()
    {
        DestroyPreviousPieces();
        GetLength();  // Determine how many pieces to spawn
        for (int i = 0; i < piecesToSpawn; i++)
        {
            SpawnPiece();

            if(i == piecesToSpawn - 1)
            {
                currentFinishLine = currentPieces[i];
                Transform trigger = currentFinishLine.transform.Find("LevelFinish");
                trigger.gameObject.SetActive(true);
            }
        }
    }

    // Spawns a level piece at the given distance from the last
    void SpawnPiece()
    {
        GameObject prefab;

        // Full pieces before level 20
        if (GameManager.Instance.level < 20)
        {
            prefab = levelPrefabs[0];
        }
        // Random pieces after level 20
        else
        {
            prefab = levelPrefabs[Random.Range(0, levelPrefabs.Length)];
        }

        Vector3 spawnPosition;

        // If there are already pieces, spawn the next one at a distance
        if (currentPieces.Count > 0)
        {
            spawnPosition = lastPosition + Vector3.forward * spawnDistance;
        }
        // First piece is spawned at Vector3.zero
        else
        {
            spawnPosition = Vector3.zero;
        }

        // Instantiate the new piece and add it to the list
        GameObject newPiece = Instantiate(prefab, spawnPosition, Quaternion.identity);
        currentPieces.Add(newPiece);

        lastPosition = spawnPosition;  // Update last spawn position
        this.transform.position = lastPosition;
    }

    // Destroy the previous level pieces
    public void DestroyPreviousPieces()
    {
        // Check if there are pieces to destroy
        if (currentPieces.Count > 1)
        {
            // Loop through all pieces except the last one
            for (int i = 0; i < currentPieces.Count - 1; i++)
            {
                Destroy(currentPieces[i]);  // Destroy each piece
            }

            // Remove all but the last piece from the list
            GameObject lastPiece = currentPieces[currentPieces.Count - 1];
            currentPieces.Clear();  // Clear the list
            currentPieces.Add(lastPiece);  // Add the last piece back into the list
        }
    }

    // Adjust the number of pieces to spawn based on the level
    public void GetLength()
    {
        switch(GameManager.Instance.level) 
        {
            case < 20:
                piecesToSpawn = 15;
                break; 
            case >= 20 and < 40:
                piecesToSpawn = 20;
                break;
            case >= 40 and < 60:
                piecesToSpawn = 25;
                break;
            case >= 60 and < 80:
                piecesToSpawn = 30;
                break;
            case >= 80:
                piecesToSpawn = 40;
                break;
        }
    }
}
