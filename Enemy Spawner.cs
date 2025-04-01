using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    
    [Header("general setting of enemy spawn")]
    public GameObject[] enemyPrefabs;
    public float spawnrate = 2.0f;
    private float spawntimer;
    public const float maxenemies = 1.0f;

    
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    public int maxEnemies = (int)maxenemies;

    
    [Header("Spawn Settings")]
    public bool useRandomPosition = false; //if true the enemy will spawn in a random posiotion
    public Vector3 spawnAreaSize = new Vector3(50f, 0f, 0f);  

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawntimer = Time.time + spawnrate;
    }

    // Update is called once per frame
    void Update()
    {
        CleanupList();
        Spawnenemy();
    }

    
    private void CleanupList()
    {
        for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
        {
            if (spawnedEnemies[i] == null)
            {
                spawnedEnemies.RemoveAt(i);
            }
        }
    }

    
    private void Spawnenemy()
    {
        if (Time.time > spawntimer && enemyPrefabs.Length > 0)
        {
            GameObject selectedEnemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Vector3 spawnPosition = transform.position;

            
            if (useRandomPosition)
            {
                spawnPosition += new Vector3(
                    Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                    0,
                    0
                );
                Debug.Log("this times a location of rocks is" + spawnPosition);
            }

            GameObject newEnemy = Instantiate(selectedEnemyPrefab, spawnPosition, transform.rotation);
            spawnedEnemies.Add(newEnemy);

            
            if (spawnedEnemies.Count > maxEnemies && spawnedEnemies[0] != null)
            {
                Destroy(spawnedEnemies[0]);
                spawnedEnemies.RemoveAt(0);
            }

            spawntimer = Time.time + spawnrate;
        }
    }
}
