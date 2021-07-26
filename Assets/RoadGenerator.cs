using System.Collections.Generic;
using UnityEngine;

public class RoadGenerator : MonoBehaviour
{
    public GameObject roadPrefab;
    // public GameObject obstaclePrefab;

    // public int maxObstacleAllowed = 4;
    // public float destroyObstacleAfter = 5.0f;

    private float zSpawn = 0;
    public float roadLength = 5;
    public int numberOfRoads = 8;

    private List<GameObject> activeTiles = new List<GameObject>();
    private List<GameObject> activeObstacle = new List<GameObject>();

    private Vector3 initialCarDirection;

    public Transform playerTransform;

    private int obstacleCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        initialCarDirection = playerTransform.position;
        for (int i = 0; i < numberOfRoads; i++)
        {
            SpawnRoad();
        }
        // InvokeRepeating("DestroyObstacle", 2.0f, destroyObstacleAfter);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform.position.z - 5 > zSpawn - (numberOfRoads * roadLength))
        {
            SpawnRoad();
            DeleteRoad();
        }
        // if(obstacleCount < maxObstacleAllowed)
        //     SpawnObstacle();
        
    }

    void SpawnRoad()
    {

        GameObject road = Instantiate(roadPrefab, new Vector3(0, 0, zSpawn), new Quaternion(0, 0, 0, 0));
        activeTiles.Add(road);
        zSpawn += roadLength;
    }

    // void SpawnObstacle()
    // {
    //     obstacleCount++;
    //     float x = Random.Range(initialCarDirection.x-7, initialCarDirection.x);
    //     float z = Random.Range(playerTransform.position.z + 6, playerTransform.position.z + 20);
    //     Vector3 obstaclePosition = new Vector3(x, initialCarDirection.y, z);
    //
    //     GameObject obstacle =  Instantiate(obstaclePrefab, obstaclePosition, new Quaternion(0, 0, 0, 0));
    //     obstacle.transform.localScale = new Vector3(0.8f, 0.6f, 0.8f);
    //     activeObstacle.Add(obstacle);
    // }


    private void DeleteRoad()
    {
        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);
    }

    // private void DestroyObstacle()
    // {
    //     foreach(var obs in activeObstacle)
    //     {
    //         Destroy(obs);
    //     }
    //     activeObstacle.Clear();
    //     obstacleCount = 0;
    // }
}
