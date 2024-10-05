using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class HillSpawn : MonoBehaviour
{

    public int currentLevel = 1;
    public int currentDifficulty = 1;
    public int levelLength = 5;
    public int hillDegree = 10;
    public float zVariance = 20;
    public Vector3 hillAngle;
    public Vector3 hillOffset;
    public List<GameObject> hillPieces;
    public List<GameObject> spawnedPieces;
    public GameObject objectSpawner;


    // Start is called before the first frame update
    void Start()
    {
        hillAngle = new Vector3(0, 0, hillDegree);
        hillOffset = new Vector2(zVariance, zVariance/5);
        CreateLevel();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateLevel()
    {
        ClearCachedPieces();
        CreateFirstPiece();
        GameManager.Instance.spawns.Clear();
        for (int i = 1; i < levelLength + 1 ; i++)
        {
            Debug.Log(i);
            LevelCreation();
            if (i % 5 == 0)
            {
                SpawnSpawner();
                Debug.Log(i + "is divisble by 5");
            }
            if (i == levelLength)
            {
                CreateLastPiece();
            }
        }

        for (int i = 0; i < spawnedPieces.Count; i++)
        {
            spawnedPieces[i].gameObject.SetActive(true);
        }
    }

    public void SetLevelAdjustments()
    {
        currentLevel = GameManager.Instance.level;
        levelLength = GameManager.Instance.length;
        hillDegree = GameManager.Instance.degree;
        currentDifficulty = GameManager.Instance.difficulty;
    }

    #region Private Methods
    private int DifficultyGenerator()
    {
        if (currentDifficulty <= 2)
        {
            return 0;
        }
        if (currentDifficulty > 2 && currentDifficulty <= 6)
        {
            int tmp = Random.Range(0, 3);
            if (tmp == 0)
            {
                int tmp2 = Random.Range(0, 2);
                switch (tmp2)
                {
                    case 0: return 1;
                    case 1: return 2;
                }
            }
            else return 0;
        }
        if (currentDifficulty > 6 && currentDifficulty < 10)
        {
            int tmp = Random.Range(0, 3);
            if (tmp != 0)
            {
                int tmp2 = Random.Range(0, 2);
                switch (tmp2)
                {
                    case 0: return 1;
                    case 1: return 2;
                }
            }
            else return 0;
        }
        if (currentDifficulty == 10)
        {
            int tmp = Random.Range(0, 2);
            switch (tmp)
            {
                case 0: return 1;
                case 1: return 2;
            }
        }
        return 0;
    }

    private void ClearCachedPieces()
    {
        if (spawnedPieces != null)
        {
            for (int i = spawnedPieces.Count - 1; i > 0; i--)
            {
                var tmp = spawnedPieces[i];
                spawnedPieces.Remove(spawnedPieces[i]);
                Destroy(tmp);

            }
        }
    }

    private void CreateFirstPiece()
    {
        GameObject firstPiece = Instantiate(hillPieces[0]);
        firstPiece.SetActive(false);
        spawnedPieces.Add(firstPiece);
        firstPiece.transform.Rotate(hillAngle);
        firstPiece.transform.position = this.transform.position;
    }

    private void LevelCreation()
    {
        int rand = DifficultyGenerator();
        GameObject tmp = Instantiate(hillPieces[rand]);
        spawnedPieces.Add(tmp);
        tmp.SetActive(false);
        tmp.transform.Rotate(hillAngle);
        this.transform.position += hillOffset;
        tmp.transform.position = this.transform.position;
        if (rand == 1)
        {
            tmp.transform.position -= new Vector3(12.5f,0,0);
        }
        else if (rand == 2)
        {
            tmp.transform.position += new Vector3(12.5f, 0, 0);
        }
    }

    private void CreateLastPiece()
    {
        Vector3 lastPiecePos = new Vector3();
        lastPiecePos = this.transform.position;
        Vector3 endOffset = new Vector2(zVariance + 2, zVariance / 10);
        GameObject endPiece = Instantiate(hillPieces[0]);
        endPiece.SetActive(false);
        endPiece.transform.position = lastPiecePos + endOffset;
        endPiece.SetActive(true);
        this.transform.position = endPiece.transform.position;
        foreach (Transform child in endPiece.transform)
        {
            if (child.TryGetComponent<BoxCollider>(out BoxCollider collider) && !collider.enabled)
            {
                collider.enabled = true;
            }
        }
    }

    private void SpawnSpawner()
    {
        GameObject nextSpawner = Instantiate(objectSpawner);
        nextSpawner.transform.position = this.transform.position + new Vector3(20f, 2, 0);
        nextSpawner.transform.Rotate(0,0,0);
    }

    #endregion
}
