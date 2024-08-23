using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public float spawnRate;
    public int spawnSpeed;
    public int tempScale;
    public List<GameObject> objects = new List<GameObject>();
    public List<Transform> spawnPoints = new List<Transform>();

    private int speedScale;
    public Vector3 speedVector;
    public List<GameObject> spawnedObjects = new List<GameObject>();
    public float timer = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        SetLevelAdjustments();
        var rotation = this.transform.rotation;
        tempScale *= spawnSpeed; 
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > spawnRate)
        {
            timer = 0;
            SpawnObject();
        }

    }

    private void SpawnObject()
    {
        Debug.Log("Spawn");
        int randomObject = Random.Range(0, objects.Count);
        int randomPoint = Random.Range(0, spawnPoints.Count);
        GameObject obj = Instantiate(objects[randomObject], spawnPoints[randomPoint].transform.position, spawnPoints[randomPoint].transform.rotation);
        spawnedObjects.Add(obj);
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        rb.AddForce(rb.transform.forward * tempScale);
    }

    public void SetLevelAdjustments()
    {
        spawnRate = GameManager.Instance.spawnRate;
        spawnSpeed = GameManager.Instance.spawnSpeed;
    }

    public void StartLevel()
    {
        this.enabled = true;
    }

    public void FinishedLevel()
    {

        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }

        // Clear the list after all objects are destroyed
        spawnedObjects.Clear();
        this.enabled = false;
    }

    public void RemoveObject(GameObject obj)
    {
        Debug.Log("Destroyed " +  obj);
        spawnedObjects.Remove(obj);
        Destroy(obj);
    }
}
