using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public Transform spawnPoint;
    public float spawnRate;
    public int spawnSpeed;
    public int tempScale;
    public List<GameObject> objects = new List<GameObject>();


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
        StartLevel();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 1)
        {
            timer = 0;
            SpawnObject();
        }

    }

    private void SpawnObject()
    {
        int randomObject = Random.Range(0, objects.Count);
        GameObject obj = Instantiate(objects[randomObject], spawnPoint.position, this.transform.rotation, this.transform);
        spawnedObjects.Add(obj);
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        rb.AddForce(rb.transform.right * -tempScale);
    }

    public void SetLevelAdjustments()
    {
        spawnRate = GameManager.Instance.spawnRate;
        spawnSpeed = GameManager.Instance.spawnSpeed;
    }

    public void StartLevel()
    {
        Debug.Log("start level");
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
